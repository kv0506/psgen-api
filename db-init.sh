#!/bin/bash
# Database initialization script for PostgreSQL on the VM

# Variables (should be changed to match your environment)
DB_NAME="psgendb"
DB_USER="psgenuser"
DB_PASSWORD="your_secure_password"

# Check if PostgreSQL is running
if ! systemctl is-active --quiet postgresql; then
    echo "PostgreSQL is not running. Starting PostgreSQL..."
    sudo systemctl start postgresql
fi

# Create database if it doesn't exist
if ! sudo -u postgres psql -lqt | cut -d \| -f 1 | grep -qw $DB_NAME; then
    echo "Creating database $DB_NAME..."
    sudo -u postgres psql -c "CREATE DATABASE $DB_NAME;"
else
    echo "Database $DB_NAME already exists."
fi

# Create user if it doesn't exist
if ! sudo -u postgres psql -c "\du" | grep -qw $DB_USER; then
    echo "Creating user $DB_USER..."
    sudo -u postgres psql -c "CREATE USER $DB_USER WITH ENCRYPTED PASSWORD '$DB_PASSWORD';"
else
    echo "User $DB_USER already exists."
fi

# Grant privileges
echo "Granting privileges on $DB_NAME to $DB_USER..."
sudo -u postgres psql -c "GRANT ALL PRIVILEGES ON DATABASE $DB_NAME TO $DB_USER;"

# Optional: Make the user a superuser (needed for some migrations)
# sudo -u postgres psql -c "ALTER USER $DB_USER WITH SUPERUSER;"

echo "Database initialization complete."
echo "Connection string for your application should be:"
echo "Host=localhost;Database=$DB_NAME;Username=$DB_USER;Password=$DB_PASSWORD"
echo "Connection string for Docker containers should be:"
echo "Host=host.docker.internal;Database=$DB_NAME;Username=$DB_USER;Password=$DB_PASSWORD"
