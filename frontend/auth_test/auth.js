// Конфигурация
const DEFAULT_API_URL = '../api';

// DOM элементы
let apiUrlInput, usernameInput, passwordInput;
let statusSpan, tokenTextarea, resultSection, curlTextarea;

// Инициализация при загрузке страницы
document.addEventListener('DOMContentLoaded', function() {
    apiUrlInput = document.getElementById('apiUrl');
    usernameInput = document.getElementById('username');
    passwordInput = document.getElementById('password');
    statusSpan = document.getElementById('status');
    tokenTextarea = document.getElementById('token');
    resultSection = document.getElementById('resultSection');
    curlTextarea = document.getElementById('curlExample');
    
    // Загружаем сохраненные данные
    loadSavedData();
    
    // Проверяем запрос от Swagger
    checkSwaggerRequest();
    
    // Обновляем ссылки и curl при изменении полей
    apiUrlInput.addEventListener('input', () => { updateLinks(); updateCurlExample(); });
    usernameInput.addEventListener('input', updateCurlExample);
    passwordInput.addEventListener('input', updateCurlExample);

    // Инициализируем пример curl
    updateCurlExample();
});

// Проверяем, пришел ли запрос от Swagger
function checkSwaggerRequest() {
    const urlParams = new URLSearchParams(window.location.search);
    const returnUrl = urlParams.get('return_url');
    
    if (returnUrl && returnUrl.includes('/swagger/')) {
        // Показываем упрощенную форму для Swagger
        document.querySelector('.auth-section').style.display = 'none';
        document.getElementById('username').value = 'admin@test.com';
        document.getElementById('password').value = 'Test1234!';
        
        // Автоматически пытаемся авторизоваться
        setTimeout(() => login(), 500);
    }
}

// Загрузка сохраненных данных из localStorage
function loadSavedData() {
    const savedApiUrl = localStorage.getItem('apiUrl');
    const savedToken = localStorage.getItem('authToken');
    
    if (savedApiUrl) apiUrlInput.value = savedApiUrl;
    if (savedToken) {
        tokenTextarea.value = savedToken;
        showResult('Токен загружен из сохранения', 'success');
        resultSection.style.display = 'block';
        updateLinks();
    }

    updateCurlExample();
}

// Сохранение данных в localStorage
function saveData(token) {
    localStorage.setItem('apiUrl', apiUrlInput.value);
    localStorage.setItem('authToken', token);
}

// Обновление ссылок
function updateLinks() {
    const baseUrl = getBaseUrl();
    const swaggerLink = document.getElementById('swaggerLink');

    if (!swaggerLink) {
        return;
    }

    // Используем наш callback механизм
    swaggerLink.href = 'javascript:void(0)';
    swaggerLink.onclick = openSwaggerWithAuth;
}

// Обновление примера curl
function updateCurlExample() {
    if (!curlTextarea) {
        return;
    }

    const apiUrl = (apiUrlInput?.value?.trim() || DEFAULT_API_URL);
    const username = (usernameInput?.value || '').replace(/"/g, '\\"');
    const password = (passwordInput?.value || '').replace(/"/g, '\\"');

    // Полный URL: относительный путь разрешаем через base
    const fullApiUrl = apiUrl.startsWith('http://') || apiUrl.startsWith('https://')
        ? apiUrl
        : new URL(apiUrl, window.location.href).href.replace(/\/?$/, '');

    const curlCommand =
`curl -X POST "${fullApiUrl}/users/login" \\
  -H "Content-Type: application/json" \\
  -d '{"email":"${username}","password":"${password}"}'`;

    curlTextarea.value = curlCommand;
}

// Генерация ссылки для Swagger OAuth callback
function generateSwaggerCallbackUrl() {
    const token = tokenTextarea.value.trim();
    
    if (!token) {
        showToast('Сначала получите токен', 'error');
        return null;
    }
    
    const currentUrl = window.location.origin + window.location.pathname;
    
    // URL для callback с токеном
    const callbackUrl = `${currentUrl}?token=${encodeURIComponent(token)}&return_url=${encodeURIComponent(getBaseUrl() + '/swagger/index.html')}`;
    
    return callbackUrl;
}

// Функция для открытия Swagger с авторизацией
function openSwaggerWithAuth() {
    const callbackUrl = generateSwaggerCallbackUrl();
    
    if (!callbackUrl) return;
    
    const baseUrl = getBaseUrl();
    
    // Открываем Swagger в новом окне/вкладке
    window.open(`${baseUrl}/swagger/index.html`, '_blank');
    
    showToast('Открываю Swagger...', 'info');
}

// Получение базового URL (без /api)
function getBaseUrl() {
    const apiUrl = apiUrlInput.value.trim() || DEFAULT_API_URL;
    // Если URL начинается с /, это относительный путь
    if (apiUrl.startsWith('/')) {
        return apiUrl.replace('/api', '');
    }
    return apiUrl.replace('/api', '');
}

// Основная функция авторизации
async function login() {
    const apiUrl = apiUrlInput.value.trim() || DEFAULT_API_URL;
    const username = usernameInput.value.trim();
    const password = passwordInput.value.trim();
    
    if (!username || !password) {
        showToast('Введите логин и пароль', 'error');
        return;
    }
    
    // Показываем состояние загрузки
    const loginBtn = document.getElementById('loginBtn');
    loginBtn.disabled = true;
    loginBtn.textContent = 'Авторизация...';
    
    try {
        const response = await fetch(`${apiUrl}/users/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify({
                email: username,
                password: password
            })
        });
        
        if (response.ok) {
            const data = await response.json();
            
            // Ответ: { token, refreshToken }
            const token = data.token || data.Token || data.accessToken || data.AccessToken;
            
            if (token) {
                tokenTextarea.value = token;
                saveData(token);
                
                // Проверяем, нужно ли перенаправить в Swagger
                const urlParams = new URLSearchParams(window.location.search);
                const returnUrl = urlParams.get('return_url');
                
                if (returnUrl && returnUrl.includes('/swagger/')) {
                    // Перенаправляем обратно в Swagger с токеном
                    const redirectUrl = `${returnUrl}#/?auth_token=${encodeURIComponent(token)}`;
                    window.location.href = redirectUrl;
                    return;
                }
                
                showResult('Успешная авторизация!', 'success');
                resultSection.style.display = 'block';
                updateLinks();
                showToast('Авторизация успешна!', 'success');
            } else {
                throw new Error('Токен не найден в ответе');
            }
        } else {
            const error = await response.text();
            throw new Error(`Ошибка ${response.status}: ${error}`);
        }
    } catch (error) {
        console.error('Login error:', error);
        showResult(`Ошибка: ${error.message}`, 'error');
        showToast(`Ошибка авторизации: ${error.message}`, 'error');
    } finally {
        loginBtn.disabled = false;
        loginBtn.textContent = 'Войти';
    }
}

// Копирование токена в буфер обмена
async function copyToken() {
    const token = tokenTextarea.value.trim();
    
    if (!token) {
        showToast('Нет токена для копирования', 'error');
        return;
    }
    
    try {
        await navigator.clipboard.writeText(token);
        showToast('Токен скопирован в буфер обмена!', 'success');
    } catch (err) {
        // Fallback для старых браузеров
        tokenTextarea.select();
        document.execCommand('copy');
        showToast('Токен скопирован!', 'success');
    }
}

// Очистка всех данных
function clearAll() {
    localStorage.removeItem('authToken');
    localStorage.removeItem('apiUrl');
    tokenTextarea.value = '';
    usernameInput.value = '';
    passwordInput.value = '';
    resultSection.style.display = 'none';
    testSection.style.display = 'none';
    apiUrlInput.value = DEFAULT_API_URL;
    showToast('Все данные очищены', 'info');
}

// Показать результат
function showResult(message, type) {
    statusSpan.textContent = message;
    statusSpan.className = `status ${type}`;
}

// Показать уведомление
function showToast(message, type) {
    const toast = document.getElementById('toast');
    toast.textContent = message;
    toast.className = `toast show ${type}`;
    
    setTimeout(() => {
        toast.className = 'toast';
    }, 3000);
}