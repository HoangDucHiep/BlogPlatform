-- Create keycloak user and database
CREATE USER keycloak WITH PASSWORD 'keycloak123';
CREATE DATABASE "KeycloakDb" OWNER keycloak;
GRANT ALL PRIVILEGES ON DATABASE "KeycloakDb" TO keycloak;