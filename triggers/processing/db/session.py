from cassandra.cluster import Cluster, ConsistencyLevel, SimpleStatement
from cassandra.auth import PlainTextAuthProvider
from ssl import CERT_NONE, PROTOCOL_TLSv1_2, SSLContext

from .connection import *

ssl_context = SSLContext(PROTOCOL_TLSv1_2)
ssl_context.verify_mode = CERT_NONE

auth_provider = PlainTextAuthProvider(username=DB_USERNAME, password=DB_PASSWORD)
cluster = Cluster([DB_ENDPOINT], port=DB_PORT, auth_provider=auth_provider, ssl_context=ssl_context)

session = cluster.connect("gamestore")
