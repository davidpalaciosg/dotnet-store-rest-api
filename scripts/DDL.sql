-- @Author: David Enrique Palacios García (paladavid@hotmail.com)
-- STORE DB DDL
-- Database: `store_db`
-- SQL script for creating the database and tables

-- Drop tables if they exist
DROP TABLE IF EXISTS `order_items`;
DROP TABLE IF EXISTS `products`;
DROP TABLE IF EXISTS `orders`;
DROP TABLE IF EXISTS `merchants`;
DROP TABLE IF EXISTS `users`;
DROP TABLE IF EXISTS `countries`;

-- store_db.countries definition

CREATE TABLE `countries` (
  `code` INT UNSIGNED AUTO_INCREMENT,
  `name` VARCHAR(100) NOT NULL,
  `continent_name` VARCHAR(100) NOT NULL,
  `state` TINYINT NOT NULL DEFAULT 1,
  PRIMARY KEY (`code`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- store_db.users definition

CREATE TABLE `users` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `full_name` varchar(150) NOT NULL,
  `email` varchar(100) NOT NULL,
  `gender` varchar(10) NOT NULL,
  `date_of_birth` date DEFAULT NULL,
  `country_code` int(10) unsigned NOT NULL,
  `created_at` date DEFAULT NULL,
  `state` tinyint(4) NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`),
  KEY `users_FK` (`country_code`),
  CONSTRAINT `users_FK` FOREIGN KEY (`country_code`) REFERENCES `countries` (`code`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- store_db.merchants definition

CREATE TABLE `merchants` (
  `id` INT UNSIGNED AUTO_INCREMENT,
  `merchant_name` VARCHAR(150) NOT NULL,
  `admin_id` INT UNSIGNED DEFAULT NULL,
  `country_code` INT UNSIGNED DEFAULT NULL,
  `created_at` DATE DEFAULT NULL,
  `state` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`),
  KEY `merchants_FK` (`admin_id`),
  KEY `merchants_FK_1` (`country_code`),
  CONSTRAINT `merchants_FK` FOREIGN KEY (`admin_id`) REFERENCES `users` (`id`),
  CONSTRAINT `merchants_FK_1` FOREIGN KEY (`country_code`) REFERENCES `countries` (`code`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- store_db.orders definition

CREATE TABLE `orders` (
  `id` INT UNSIGNED AUTO_INCREMENT,
  `user_id` INT UNSIGNED DEFAULT NULL,
  `status` VARCHAR(100) NOT NULL,
  `created_at` DATE DEFAULT NULL,
  `state` TINYINT NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`),
  KEY `orders_FK` (`user_id`),
  CONSTRAINT `orders_FK` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- store_db.products definition

CREATE TABLE `products` (
  `id` INT UNSIGNED AUTO_INCREMENT,
  `merchant_id` INT UNSIGNED DEFAULT NULL,
  `name` VARCHAR(100) NOT NULL,
  `price` DOUBLE NOT NULL,
  `status` VARCHAR(100) NOT NULL,
  `created_at` DATE DEFAULT NULL,
  `state` TINYINT NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`),
  KEY `products_FK` (`merchant_id`),
  CONSTRAINT `products_FK` FOREIGN KEY (`merchant_id`) REFERENCES `merchants` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- store_db.order_items definition

CREATE TABLE `order_items` (
  `order_id` INT UNSIGNED NOT NULL,
  `product_id` INT UNSIGNED NOT NULL,
  `quantity` INT UNSIGNED DEFAULT 0,
  `state` TINYINT NOT NULL DEFAULT 1,
  PRIMARY KEY (`order_id`,`product_id`),
  KEY `order_items_FK_1` (`product_id`),
  CONSTRAINT `order_items_FK` FOREIGN KEY (`order_id`) REFERENCES `orders` (`id`),
  CONSTRAINT `order_items_FK_1` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


