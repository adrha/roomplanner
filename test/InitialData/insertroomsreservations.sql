-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               10.7.1-MariaDB-1:10.7.1+maria~focal - mariadb.org binary distribution
-- Server OS:                    debian-linux-gnu
-- HeidiSQL Version:             12.0.0.6468
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

-- Dumping data for table roomplanner.RoomReservations: ~3 rows (approximately)
INSERT INTO `RoomReservations` (`Id`, `UserId`, `RoomId`, `From`, `To`, `Subject`) VALUES
	('1085caea-39c2-41ce-98f9-354c40d05643', 'd7d4ca01-7449-4a7f-bf1a-d4502cb50619', '166aae54-a34a-4fdf-a568-63d60062c3e3', '2022-07-12 08:00:00.000000', '2022-07-12 09:00:00.000000', 'Bewerbungsgespräch Nina Tunte'),
	('1790f782-a37c-4173-bfb8-a3af02ad60fc', 'd7d4ca01-7449-4a7f-bf1a-d4502cb50619', '9bf6893b-8611-41f0-9d8f-a7090339403f', '2022-07-12 10:00:00.000000', '2022-07-12 11:30:00.000000', 'Kündigungsgespräch Hans Nötig'),
	('37576f31-8700-43a8-8103-a1510600ddc4', 'd7d4ca01-7449-4a7f-bf1a-d4502cb50619', '166aae54-a34a-4fdf-a568-63d60062c3e3', '2022-07-12 09:00:00.000000', '2022-07-12 10:00:00.000000', 'Bewerbungsgespräch P.Escobar'),
	('7b0b26af-48d8-4be8-894d-36656cebc842', 'd7d4ca01-7449-4a7f-bf1a-d4502cb50619', 'b45e7355-e698-44a9-ada7-4389d8c23a5f', '2022-07-12 08:38:27.000000', '2022-07-12 15:38:33.000000', 'Bastelverein Gammelbach'),
	('b182eeff-5919-4cdd-be30-9307b30a6cec', 'd7d4ca01-7449-4a7f-bf1a-d4502cb50619', '6eb9dbf1-3d0a-4e13-b836-2ff09ed8e65f', '2022-07-12 11:00:00.000000', '2022-07-12 14:00:00.000000', 'Weiterbildung Marketing Klasse 3b'),
	('e402922b-876d-4a00-a253-8cc2c295c616', 'd7d4ca01-7449-4a7f-bf1a-d4502cb50619', '166aae54-a34a-4fdf-a568-63d60062c3e3', '2022-07-12 14:00:00.000000', '2022-07-12 18:00:00.000000', 'Bila Hans und Diethelm');

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
