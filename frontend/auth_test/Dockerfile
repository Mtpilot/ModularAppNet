# Use lightweight nginx image
FROM nginx:alpine

# Install build dependencies (if needed)
RUN apk update && apk add --no-cache nodejs npm

# Create working directory
WORKDIR /app

# Copy project files
COPY index.html /usr/share/nginx/html/
COPY style.css /usr/share/nginx/html/
COPY auth.js /usr/share/nginx/html/

# Copy nginx configuration
COPY nginx.conf /etc/nginx/nginx.conf

# Create healthcheck
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:80/ || exit 1

# Expose port 80
EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"]