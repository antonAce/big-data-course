CREATE TYPE IF NOT EXISTS gamestore.price_record
(
	date_price timestamp,
	initial_price double,
	final_price double,
	discount_on_price int
);

CREATE TABLE IF NOT EXISTS gamestore.users
(
    nickname           varchar,
    steam_profile_link varchar,
    wishlist           list<varchar>,
    owned_games        list<varchar>,
    friends            list<varchar>,

    PRIMARY KEY (nickname)
);

CREATE TABLE IF NOT EXISTS gamestore.games
(
    app_id        int,
    name          varchar,
    description   varchar,
    origin        varchar,
    genres        list<varchar>,
    developers    list<varchar>,
    release_date  timestamp,
    price_history list<FROZEN<price_record>>,

    PRIMARY KEY (app_id, name)
)
WITH CLUSTERING ORDER BY (name ASC);

CREATE TABLE IF NOT EXISTS gamestore.comments
(
    id          uuid,
    app_id      int,
    nickname    varchar,
    date_time   timestamp,
    containment varchar,
    reaction    varchar,

    PRIMARY KEY (id, app_id, nickname)
)
WITH CLUSTERING ORDER BY (app_id ASC);
