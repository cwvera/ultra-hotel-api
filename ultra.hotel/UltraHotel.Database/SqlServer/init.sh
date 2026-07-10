#!/bin/bash
set -e

MAX_RETRIES=20
RETRY=0

echo "[SQL] Waiting for SQL Server to accept connections..."

until /opt/mssql-tools18/bin/sqlcmd \
        -S sqlserver -U sa -P "${SA_PASSWORD}" \
        -Q "SELECT 1" -b -C \
        > /dev/null 2>&1; do
    RETRY=$((RETRY + 1))
    if [ "$RETRY" -ge "$MAX_RETRIES" ]; then
        echo "[SQL] ERROR: SQL Server not available after $MAX_RETRIES attempts. Aborting."
        exit 1
    fi
    echo "[SQL] Not ready yet (attempt $RETRY/$MAX_RETRIES). Retrying in 5s..."
    sleep 5
done

echo "[SQL] Connection accepted. Waiting 8s for system databases to finish recovery..."
sleep 8

echo "[SQL] Running schema..."

/opt/mssql-tools18/bin/sqlcmd \
    -S sqlserver \
    -U sa \
    -P "${SA_PASSWORD}" \
    -d master \
    -i /scripts/schema.sql \
    -b -C

echo "[SQL] Schema initialized. Running seed data..."

/opt/mssql-tools18/bin/sqlcmd \
    -S sqlserver \
    -U sa \
    -P "${SA_PASSWORD}" \
    -d master \
    -i /scripts/seed.sql \
    -b -C

echo "[SQL] Seed data loaded. Initialization complete."
