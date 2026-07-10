#!/bin/sh
# Inserta documentos semilla de habitaciones en Elasticsearch
# Los IDs coinciden exactamente con los de seed.sql

ES_URL="http://elasticsearch:9200"
INDEX="hotel_$(date +%Y%m)"

echo "[ES-SEED] Indexing dummy room documents into $INDEX..."

# ─── Hotel Casa Bogotá — Room 201 SINGLE ────────────────────────────────────
curl -s -o /dev/null -w "[ES-SEED] Room 201 Bogota SINGLE: %{http_code}\n" \
  -X PUT "${ES_URL}/${INDEX}/_doc/B1B1B1B1-0000-0000-0000-000000000001" \
  -H "Content-Type: application/json" -d '{
    "hotel_id":        "A1A1A1A1-0000-0000-0000-000000000001",
    "hotel_name":      "Hotel Casa Bogotá",
    "city":            "bogotá",
    "room_id":         "B1B1B1B1-0000-0000-0000-000000000001",
    "room_type":       "SINGLE",
    "capacity":        1,
    "base_price":      120000.00,
    "tax_rate":        0.19,
    "location_in_hotel": "Piso 2, habitación 201",
    "is_available":    true,
    "hotel_enabled":   true,
    "room_enabled":    true,
    "year_month":      "'"$(date +%Y%m)"'"
  }'

# ─── Hotel Casa Bogotá — Room 301 DOUBLE ────────────────────────────────────
curl -s -o /dev/null -w "[ES-SEED] Room 301 Bogota DOUBLE: %{http_code}\n" \
  -X PUT "${ES_URL}/${INDEX}/_doc/B1B1B1B1-0000-0000-0000-000000000002" \
  -H "Content-Type: application/json" -d '{
    "hotel_id":        "A1A1A1A1-0000-0000-0000-000000000001",
    "hotel_name":      "Hotel Casa Bogotá",
    "city":            "bogotá",
    "room_id":         "B1B1B1B1-0000-0000-0000-000000000002",
    "room_type":       "DOUBLE",
    "capacity":        2,
    "base_price":      200000.00,
    "tax_rate":        0.19,
    "location_in_hotel": "Piso 3, habitación 301",
    "is_available":    true,
    "hotel_enabled":   true,
    "room_enabled":    true,
    "year_month":      "'"$(date +%Y%m)"'"
  }'

# ─── Hotel Casa Bogotá — Suite presidencial ──────────────────────────────────
curl -s -o /dev/null -w "[ES-SEED] Suite Bogota: %{http_code}\n" \
  -X PUT "${ES_URL}/${INDEX}/_doc/B1B1B1B1-0000-0000-0000-000000000003" \
  -H "Content-Type: application/json" -d '{
    "hotel_id":        "A1A1A1A1-0000-0000-0000-000000000001",
    "hotel_name":      "Hotel Casa Bogotá",
    "city":            "bogotá",
    "room_id":         "B1B1B1B1-0000-0000-0000-000000000003",
    "room_type":       "SUITE",
    "capacity":        3,
    "base_price":      450000.00,
    "tax_rate":        0.19,
    "location_in_hotel": "Piso 10, suite presidencial",
    "is_available":    true,
    "hotel_enabled":   true,
    "room_enabled":    true,
    "year_month":      "'"$(date +%Y%m)"'"
  }'

# ─── Hotel El Poblado Medellín — Room 401 FAMILY ────────────────────────────
curl -s -o /dev/null -w "[ES-SEED] Room 401 Medellin FAMILY: %{http_code}\n" \
  -X PUT "${ES_URL}/${INDEX}/_doc/B2B2B2B2-0000-0000-0000-000000000001" \
  -H "Content-Type: application/json" -d '{
    "hotel_id":        "A2A2A2A2-0000-0000-0000-000000000002",
    "hotel_name":      "Hotel El Poblado Medellín",
    "city":            "medellín",
    "room_id":         "B2B2B2B2-0000-0000-0000-000000000001",
    "room_type":       "FAMILY",
    "capacity":        4,
    "base_price":      380000.00,
    "tax_rate":        0.19,
    "location_in_hotel": "Piso 4, habitación 401",
    "is_available":    true,
    "hotel_enabled":   true,
    "room_enabled":    true,
    "year_month":      "'"$(date +%Y%m)"'"
  }'

# ─── Hotel El Poblado Medellín — Room 204 DOUBLE ────────────────────────────
curl -s -o /dev/null -w "[ES-SEED] Room 204 Medellin DOUBLE: %{http_code}\n" \
  -X PUT "${ES_URL}/${INDEX}/_doc/B2B2B2B2-0000-0000-0000-000000000002" \
  -H "Content-Type: application/json" -d '{
    "hotel_id":        "A2A2A2A2-0000-0000-0000-000000000002",
    "hotel_name":      "Hotel El Poblado Medellín",
    "city":            "medellín",
    "room_id":         "B2B2B2B2-0000-0000-0000-000000000002",
    "room_type":       "DOUBLE",
    "capacity":        2,
    "base_price":      210000.00,
    "tax_rate":        0.19,
    "location_in_hotel": "Piso 2, habitación 204",
    "is_available":    true,
    "hotel_enabled":   true,
    "room_enabled":    true,
    "year_month":      "'"$(date +%Y%m)"'"
  }'

echo "[ES-SEED] All room documents indexed."
