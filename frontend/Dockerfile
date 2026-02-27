# Use lightweight nginx image
FROM nginx:alpine

# Copy static files
COPY index.html /usr/share/nginx/html/
COPY readme-ru.html /usr/share/nginx/html/
COPY auth_test/ /usr/share/nginx/html/auth_test/

# Create healthcheck
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:80/ || exit 1

# Expose port 80
EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"]
