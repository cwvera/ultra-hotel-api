#!/bin/sh
set -e

ES_URL="http://elasticsearch:9200"
MAX_RETRIES=15
RETRY=0

echo "[ES] Waiting for Elasticsearch..."

until curl -s "${ES_URL}/_cluster/health" > /dev/null 2>&1; do
    RETRY=$((RETRY + 1))
    if [ "$RETRY" -ge "$MAX_RETRIES" ]; then
        echo "[ES] ERROR: Elasticsearch not available after $MAX_RETRIES attempts. Aborting."
        exit 1
    fi
    echo "[ES] Not ready yet (attempt $RETRY/$MAX_RETRIES). Retrying in 5s..."
    sleep 5
done

echo "[ES] Registering hotel index template (hotel_*)..."

HTTP_STATUS=$(curl -s -o /tmp/es_response.json -w "%{http_code}" \
    -X PUT "${ES_URL}/_index_template/hotel_template" \
    -H "Content-Type: application/json" \
    -d "@/scripts/hotel_template.json")

cat /tmp/es_response.json
echo ""

if [ "$HTTP_STATUS" -eq 200 ] || [ "$HTTP_STATUS" -eq 201 ]; then
    echo "[ES] Template registered successfully (HTTP $HTTP_STATUS)."
else
    echo "[ES] ERROR: Template registration failed (HTTP $HTTP_STATUS)."
    exit 1
fi

echo "[ES] Seeding dummy room documents..."
sh /scripts/es_seed.sh

echo "[ES] Elasticsearch initialization complete."
