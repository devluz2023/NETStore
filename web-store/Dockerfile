# Use an official Node image to build the Angular app
FROM node:18-alpine AS build

# Set working directory
WORKDIR /app

# Copy package.json and package-lock.json
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of the application code
COPY . .

# Build the app
RUN npm run build --prod

# Use Nginx to serve the build
FROM nginx:alpine

# Copy the built files from the previous stage
COPY --from=build /app/dist/developer-store-frontend /usr/share/nginx/html

# Optional: overwrite default nginx config
# COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expose port 80
EXPOSE 80

# Start Nginx server
CMD ["nginx", "-g", "daemon off;"]