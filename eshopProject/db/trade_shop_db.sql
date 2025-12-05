-- création de la base de données
create database trade_shop_db;
go
use trade_shop_db;
go

-- table users
create table users (
    user_id int primary key identity(1,1),
    username varchar(50) not null unique,
    email varchar(100) not null unique,
    password varchar(255) not null,
    role varchar(50) check (role in ('connected_user', 'admin')) default 'connected_user',
    profile_picture varchar(255),
    membership_level varchar(50),
    rating float default 0,
    status varchar(50) check (status in ('active', 'suspended', 'deleted')) default 'active',
    balance decimal(10,2) default 0 not null
);

-- table articles
create table articles (
    article_id int primary key identity(1,1),
    title varchar(100) not null,
    description varchar(max),
    price decimal(10, 2) not null,
    category varchar(50),
    state varchar(50) check (state in ('new', 'used')) default 'used',
    user_id int,
    created_at datetime default current_timestamp,
    updated_at datetime default current_timestamp,
    status varchar(50) check (status in ('available', 'sold', 'removed')) default 'available',
    main_image_url varchar(255),
    quantity int default 1 not null
);

-- table transactions
create table transactions (
    transaction_id int primary key identity(1,1),
    buyer_id int,
    seller_id int,
    article_id int,
    transaction_type varchar(50) check (transaction_type in ('purchase', 'exchange')) not null,
    price decimal(10, 2) not null,
    commission decimal(10, 2) default 10.00,
    transaction_date datetime default current_timestamp,
    status varchar(50) check (status in ('in progress', 'finished', 'cancelled')) default 'in progress'
);

-- table trades (échanges)
create table trades (
    trade_id int primary key identity(1,1),
    trader_id int,
    receiver_id int,
    trader_articles_ids varchar(max),
    receiver_article_id int,
    trade_date datetime default current_timestamp,
    status varchar(50) check (status in ('in progress', 'accepted', 'denied')) default 'in progress'
);

-- table memberships
create table memberships (
    membership_id int primary key identity(1,1),
    name varchar(50) not null unique,
    price decimal(10, 2) not null,
    discount_percentage decimal(5, 2) default 5.00,
    description varchar(max)
);

-- table user_memberships
create table user_memberships (
    user_membership_id int primary key identity(1,1),
    user_id int,
    membership_id int,
    start_date datetime default current_timestamp,
    end_date datetime,
    status varchar(50) check (status in ('active', 'expired', 'canceled')) default 'active'
);

-- table ratings
create table ratings (
    rating_id int primary key identity(1,1),
    user_id int,
    reviewer_id int,
    score int check (score between 1 and 5),
    comment varchar(max),
    created_at datetime default current_timestamp
);

-- ajout des clés étrangères pour la table articles
alter table articles
    add constraint fk_articles_user foreign key (user_id) references users(user_id);

-- ajout des clés étrangères pour la table transactions
alter table transactions
    add constraint fk_transactions_buyer foreign key (buyer_id) references users(user_id);

alter table transactions
    add constraint fk_transactions_seller foreign key (seller_id) references users(user_id);

alter table transactions
    add constraint fk_transactions_article foreign key (article_id) references articles(article_id);

-- ajout des clés étrangères pour la table trades
alter table trades
    add constraint fk_trades_user1 foreign key (trader_id) references users(user_id);

alter table trades
    add constraint fk_trades_user2 foreign key (receiver_id) references users(user_id);

alter table trades
    add constraint fk_trades_article2 foreign key (receiver_article_id) references articles(article_id);

-- ajout des clés étrangères pour la table user_memberships
alter table user_memberships
    add constraint fk_user_memberships_user foreign key (user_id) references users(user_id);

alter table user_memberships
    add constraint fk_user_memberships_membership foreign key (membership_id) references memberships(membership_id);

-- ajout des clés étrangères pour la table ratings
alter table ratings
    add constraint fk_ratings_user foreign key (user_id) references users(user_id);

alter table ratings
    add constraint fk_ratings_reviewer foreign key (reviewer_id) references users(user_id);


insert into Memberships (name, price, discount_percentage, description)
values('Bronze', 2, 0.25, 'Unlock the benefits of our BRONZE membership! Enjoy a 25% reduction on commission fees, helping you save more with every sale. Perfect for those looking to boost their profits without breaking the bank'),('Silver', 3, 0.50, 'Step up to the SILVER membership and enjoy an impressive 50% reduction on commission fees. Designed for ambitious sellers, this plan helps you maximize your earnings while staying cost-effective.'),('Gold', 5, 1.00, 'Experience the ultimate freedom with our GOLD membership! Say goodbye to commission fees entirely and keep 100% of your profits. The perfect choice for top-tier sellers who demand the best.');


INSERT INTO users (username, email, password, role, profile_picture, membership_level, rating, status, balance)
VALUES
    ('admin', 'admin@helha.be', '77e467eb0169e82e77f090df217a323357c6a157a98c0375e6f6dbafe029c83a', 'admin', 'profilePic/uploads\8f01238f-11d6-4254-9fcb-c45b65b5b875.png', 'Gold', 5.0, 'active', 1000.0);

INSERT INTO users (username, email, password, role, profile_picture, membership_level, rating, status, balance)
VALUES
    ('user', 'user@helha.be', 'c7c66cfbf2472170ca65705e6711ab7d681d9da1ba7803283540d0b0509e97aa', 'connected_user', 'profilePic/uploads\3c854e93-8909-4a87-9eb4-c4565ac4e9ff.png', 'Gold', 5.0, 'active', 500.0);

--articles
INSERT INTO articles (title, description, price, category, state, user_id, created_at, updated_at, status, main_image_url, quantity)
VALUES
('Crocks', 'A pair of crocks', 12.00, 'Clothing', 'new', 1, '2024-12-20 00:00:00.000', '2024-12-20 21:36:45.847', 'available', 'artImages/uploads\\686729cb-45fe-461b-a0e0-6036fb7a8815.png', 1),
('Cereal Bar', 'A pack of Cereal Bar', 5.00, 'Food', 'new', 1, '2024-12-20 00:00:00.000', '2024-12-20 20:38:21.317', 'available', 'artImages/uploads\\85de3464-9c1b-40f4-bb49-cd56204a1b8a.png', 2),
('Wooden chair', 'A wooden chair with some nice leather', 80.00, 'Furnishing', 'new', 1, '2024-12-20 00:00:00.000', '2024-12-20 20:39:02.800', 'available', 'artImages/uploads\\ef91bccf-3cc7-4d2d-97ac-38c073ad84e7.png', 1),
('Dumbbell', 'A kit of dumbbell', 36.00, 'Sports', 'used', 1, '2024-12-20 00:00:00.000', '2024-12-20 20:39:43.933', 'available', 'artImages/uploads\\6fadb612-0dde-45e0-a1a6-8ae51b187728.png', 1),
('Gardening tool', 'A set of gardening tool', 14.00, 'Other', 'new', 1, '2024-12-20 00:00:00.000', '2024-12-20 20:42:05.417', 'available', 'artImages/uploads\\92ca04c9-38a2-4067-86f3-86a035296407.png', 1),
('Makeup brushed', 'A nice kit of makeup brushes', 17.00, 'Beauty', 'new', 1, '2024-12-20 00:00:00.000', '2024-12-20 20:42:56.387', 'available', 'artImages/uploads\\b4147ae6-30fb-42eb-9619-490690c27379.png', 1),
('Gaming mouse', 'A perfect mouse for gamer', 60.00, 'Electronics', 'new', 2, '2024-12-20 00:00:00.000', '2024-12-20 20:45:27.433', 'available', 'artImages/uploads\\bd69bb6f-a29f-47cc-9df6-cd4793e35023.png', 1),
('Pop Pirate tabletob', 'A nice tabletop game for kids', 19.00, 'Toys', 'new', 2, '2024-12-20 00:00:00.000', '2024-12-20 20:46:14.103', 'available', 'artImages/uploads\\d215b176-cea8-41d0-8fed-da1e4da09603.png', 1),
('Power cell', 'A pack of 20 power cell', 6.00, 'Electronics', 'new', 2, '2024-12-20 00:00:00.000', '2024-12-20 20:47:00.733', 'available', 'artImages/uploads\\ca7e038c-5ac2-4d6c-b416-40dcd274f5b4.png', 2),
('Tyre inflater', 'To inflate your tyre this one is perfect', 43.00, 'Vehicles', 'used', 2, '2024-12-20 00:00:00.000', '2024-12-20 20:49:32.503', 'available', 'artImages/uploads\\b1204a9d-f289-4f9b-a392-66fa9f1958a0.png', 1),
('A watch', 'A watch with a green background', 9.00, 'Electronics', 'used', 2, '2024-12-20 00:00:00.000', '2024-12-20 20:50:27.327', 'available', 'artImages/uploads\\abc48086-9d36-4760-a62d-d3a63bc9cbfb.png', 1),
('Wayward Pine', 'Wayward Pine by blake rouch', 25.00, 'Books', 'new', 2, '2024-12-20 00:00:00.000', '2024-12-20 20:51:56.750', 'available', 'artImages/uploads\\d5f80729-8a03-4027-b6f7-e2913df4523b.png', 1),
('another mouse1', 'another mouse1', 50.00, 'Electronics', 'new', 1, '2024-12-20 00:00:00.000', '2024-12-20 20:53:32.317', 'available', 'artImages/uploads\\def44e15-a65b-4882-82f0-572357794bb2.png', 1),
('another mouse2', 'another mouse2', 50.00, 'Electronics', 'new', 1, '2024-12-20 00:00:00.000', '2024-12-20 20:54:33.347', 'available', 'artImages/uploads\\62d627f7-8904-4fb1-8558-a2b6ab6c09aa.png', 1),
('another mouse3', 'another mouse3', 50.00, 'Electronics', 'new', 1, '2024-12-20 00:00:00.000', '2024-12-20 20:55:01.823', 'available', 'artImages/uploads\\956c3bb7-7a02-44e1-951f-a88ee9c20661.png', 1),
('another mouse4', 'another mouse4', 50.00, 'Electronics', 'new', 1, '2024-12-20 00:00:00.000', '2024-12-20 20:56:39.870', 'available', 'artImages/uploads\\47d44916-dcb5-46f0-84f2-6f4c0e2efea3.png', 1),
('another mouse6', 'another mouse6', 30.00, 'Electronics', 'new', 1, '2024-12-20 00:00:00.000', '2024-12-20 20:57:00.703', 'available', 'artImages/uploads\\cb146647-b715-43f6-9066-7d445a62a0cb.png', 1);













