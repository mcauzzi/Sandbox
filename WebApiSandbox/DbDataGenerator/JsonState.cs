public record JsonState(
    int    id,
    string name,
    int    country_id,
    string country_code,
    string country_name,
    string state_code,
    object type,
    string latitude,
    string longitude
);