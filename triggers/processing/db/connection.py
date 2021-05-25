import os

DB_USERNAME = os.getenv('ENV_DB_USERNAME')
DB_PASSWORD = os.getenv('ENV_DB_PASSWORD')

DB_ENDPOINT = os.getenv('ENV_DB_ENDPOINT')
DB_PORT = int(os.getenv('ENV_DB_PORT'))
