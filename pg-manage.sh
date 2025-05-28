#!/bin/bash
# PostgreSQL management script for the Hetzner VM

ACTION=$1
DB_NAME="psgendb"
DB_USER="psgenuser"
DB_PASSWORD=${2:-"your_secure_password"}  # Default password or use arg 2

function show_usage {
  echo "Usage: $0 [action] [optional_password]"
  echo "Actions:"
  echo "  status      - Show PostgreSQL status"
  echo "  backup      - Backup the database"
  echo "  restore     - Restore from the latest backup"
  echo "  reset-pw    - Reset the database user password"
  echo "  check-conn  - Check connection from Docker network"
  echo "  config      - Show PostgreSQL configuration"
  echo "  logs        - Show PostgreSQL logs"
  echo "  optimize    - Run vacuum analyze on the database"
  echo "  help        - Show this help message"
}

function check_postgres {
  if ! systemctl is-active --quiet postgresql; then
    echo "PostgreSQL is not running. Starting PostgreSQL..."
    sudo systemctl start postgresql
    sleep 2
  fi
}

case "$ACTION" in
  status)
    sudo systemctl status postgresql
    ;;
    
  backup)
    check_postgres
    BACKUP_FILE="psgendb_backup_$(date +%Y%m%d_%H%M%S).sql"
    echo "Creating backup: $BACKUP_FILE"
    sudo -u postgres pg_dump $DB_NAME > ~/$BACKUP_FILE
    echo "Backup completed: ~/$BACKUP_FILE"
    ;;
    
  restore)
    check_postgres
    LATEST_BACKUP=$(ls -t ~/psgendb_backup_*.sql | head -1)
    if [ -z "$LATEST_BACKUP" ]; then
      echo "No backup files found."
      exit 1
    fi
    echo "Restoring from backup: $LATEST_BACKUP"
    sudo -u postgres psql -c "DROP DATABASE IF EXISTS $DB_NAME;"
    sudo -u postgres psql -c "CREATE DATABASE $DB_NAME;"
    sudo -u postgres psql -c "GRANT ALL PRIVILEGES ON DATABASE $DB_NAME TO $DB_USER;"
    sudo -u postgres psql $DB_NAME < $LATEST_BACKUP
    echo "Restore completed."
    ;;
    
  reset-pw)
    check_postgres
    echo "Resetting password for user $DB_USER to $DB_PASSWORD"
    sudo -u postgres psql -c "ALTER USER $DB_USER WITH PASSWORD '$DB_PASSWORD';"
    echo "Password reset successful."
    echo "Don't forget to update your secrets in GitHub and docker-compose.yml"
    ;;
    
  check-conn)
    check_postgres
    # Create a temporary container in the Docker network to test PostgreSQL connection
    echo "Creating a temporary PostgreSQL client container to test connection..."
    docker run --rm --name pg-test \
      --network="host" \
      postgres:latest \
      psql "host=localhost dbname=$DB_NAME user=$DB_USER password=$DB_PASSWORD" \
      -c "SELECT 'Connection successful' AS status;"
    
    echo "Testing connection from container with host.docker.internal..."
    docker run --rm --name pg-test \
      --add-host=host.docker.internal:host-gateway \
      postgres:latest \
      psql "host=host.docker.internal dbname=$DB_NAME user=$DB_USER password=$DB_PASSWORD" \
      -c "SELECT 'Connection successful' AS status;"
    ;;
    
  config)
    echo "PostgreSQL configuration:"
    sudo grep -v "^#" /etc/postgresql/*/main/postgresql.conf | grep -v "^$"
    echo -e "\nPostgreSQL host-based authentication:"
    sudo grep -v "^#" /etc/postgresql/*/main/pg_hba.conf | grep -v "^$"
    ;;
    
  logs)
    sudo tail -n 100 /var/log/postgresql/postgresql-*-main.log
    ;;
    
  optimize)
    check_postgres
    echo "Running vacuum analyze on $DB_NAME..."
    sudo -u postgres psql -c "VACUUM ANALYZE $DB_NAME;"
    echo "Database optimization completed."
    ;;
    
  help|*)
    show_usage
    ;;
esac
