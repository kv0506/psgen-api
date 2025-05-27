#!/bin/bash
# Server setup script for Hetzner Ubuntu VM

# Update system
echo "Updating system packages..."
sudo apt-get update
sudo apt-get upgrade -y

# Install Docker and Docker Compose
echo "Installing Docker..."
sudo apt-get install -y apt-transport-https ca-certificates curl software-properties-common
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -
sudo add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable"
sudo apt-get update
sudo apt-get install -y docker-ce docker-ce-cli containerd.io

echo "Installing Docker Compose..."
sudo curl -L "https://github.com/docker/compose/releases/download/v2.20.0/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Add current user to docker group
echo "Adding user to docker group..."
sudo usermod -aG docker $USER

# Install Nginx
echo "Installing Nginx..."
sudo apt-get install -y nginx

# Create application directory
echo "Creating application directory..."
mkdir -p ~/psgen-api

# Copy Nginx configuration
echo "Setting up Nginx configuration..."
sudo cp nginx.conf /etc/nginx/sites-available/psgen-api
sudo ln -s /etc/nginx/sites-available/psgen-api /etc/nginx/sites-enabled/
sudo rm /etc/nginx/sites-enabled/default

# Install Certbot for SSL
echo "Installing Certbot for SSL..."
sudo apt-get install -y certbot python3-certbot-nginx

# Set up systemd service
echo "Setting up systemd service..."
sudo cp psgen-api.service /etc/systemd/system/
sudo systemctl daemon-reload
sudo systemctl enable psgen-api.service

echo "Server setup complete. Next steps:"
echo "1. Update the Nginx configuration with your domain name"
echo "2. Get SSL certificate: sudo certbot --nginx -d yourdomain.com"
echo "3. Start the service: sudo systemctl start psgen-api"
