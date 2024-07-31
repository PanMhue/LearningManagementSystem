/*
SQLyog Community v13.2.1 (64 bit)
MySQL - 8.4.0 : Database - ojtprogramdb
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
CREATE DATABASE /*!32312 IF NOT EXISTS*/`ojtprogramdb` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;

USE `ojtprogramdb`;

/*Table structure for table `course_assignments` */

DROP TABLE IF EXISTS `course_assignments`;

CREATE TABLE `course_assignments` (
  `AssignmentID` int NOT NULL AUTO_INCREMENT,
  `CourseID` int NOT NULL,
  `UserID` int NOT NULL,
  PRIMARY KEY (`AssignmentID`),
  KEY `UserID` (`UserID`),
  KEY `idx_course_user` (`CourseID`,`UserID`),
  CONSTRAINT `course_assignments_ibfk_1` FOREIGN KEY (`CourseID`) REFERENCES `courses` (`CourseID`),
  CONSTRAINT `course_assignments_ibfk_2` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `course_assignments` */

LOCK TABLES `course_assignments` WRITE;

insert  into `course_assignments`(`AssignmentID`,`CourseID`,`UserID`) values 
(1,1,3),
(17,1,5),
(24,1,6),
(11,1,7),
(14,2,3),
(18,2,5),
(25,2,6),
(21,2,7),
(3,3,3),
(19,3,5),
(7,3,7),
(4,4,3),
(5,4,5),
(6,4,7),
(22,5,3),
(23,5,5),
(13,5,7),
(15,6,3),
(16,6,5),
(10,6,7);

UNLOCK TABLES;

/*Table structure for table `courses` */

DROP TABLE IF EXISTS `courses`;

CREATE TABLE `courses` (
  `CourseID` int NOT NULL AUTO_INCREMENT,
  `CourseName` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Description` text COLLATE utf8mb4_unicode_ci,
  `CreatedAt` datetime NOT NULL,
  PRIMARY KEY (`CourseID`),
  UNIQUE KEY `CourseName` (`CourseName`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `courses` */

LOCK TABLES `courses` WRITE;

insert  into `courses`(`CourseID`,`CourseName`,`Description`,`CreatedAt`) values 
(1,'Math 101','Introduction to Mathematics','2024-07-24 14:51:24'),
(2,'Science 101','Introduction to Science','2024-07-24 14:51:24'),
(3,'OJT','CSS','0001-01-01 00:00:00'),
(4,'Coding','html','0001-01-01 00:00:00'),
(5,'accounting','Lcci','0001-01-01 00:00:00'),
(6,'accounting3','Lcci3','0001-01-01 00:00:00');

UNLOCK TABLES;

/*Table structure for table `event_log` */

DROP TABLE IF EXISTS `event_log`;

CREATE TABLE `event_log` (
  `log_type` int NOT NULL COMMENT '0 = info, 1 = Warning, 2 = Error, 3 = Insert, 4 = Update, 5 = Delete',
  `log_datetime` datetime NOT NULL,
  `log_message` text COLLATE utf8mb4_unicode_ci,
  `error_message` text COLLATE utf8mb4_unicode_ci,
  `form_name` varchar(500) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'function name',
  `source` varchar(500) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'controller name'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `event_log` */

LOCK TABLES `event_log` WRITE;

insert  into `event_log`(`log_type`,`log_datetime`,`log_message`,`error_message`,`form_name`,`source`) values 
(3,'2024-07-29 23:33:50','Created a new data User ID: 7 viewed their courses.','','ViewMyCourses','CourseController'),
(2,'2024-07-29 23:36:36','Error occurred while fetching courses','Test Exception','ViewMyCourses','CourseController'),
(3,'2024-07-30 14:29:58','Created a new data User ID: 7 viewed their courses.','','ViewMyCourses','CourseController'),
(3,'2024-07-30 19:33:22','Created a new data User ID: 7 viewed their courses.','','ViewMyCourses','CourseController'),
(3,'2024-07-30 19:37:13','Created a new data Course created: accounting','','CreateCourse','CourseController'),
(3,'2024-07-30 19:39:26','Created a new data Exercise created: ssss','','CreateExercise','ExerciseController'),
(3,'2024-07-30 20:17:22','Created a new data Exercise ID: 4 marked for Student ID: 3','','MarkExercise','ExerciseController'),
(3,'2024-07-30 20:17:45','Created a new data User ID: 3 viewed their exercises.','','ViewMyExercises','ExerciseController'),
(3,'2024-07-30 20:22:35','Created a new data User ID: 3 viewed their exercises.','','ViewMyExercises','ExerciseController'),
(3,'2024-07-30 21:19:33','Created a new data User ID: 3 viewed their exercises.','','ViewMyExercises','ExerciseController'),
(3,'2024-07-30 21:20:55','Created a new data User ID: 7 viewed their exercises.','','ViewMyExercises','ExerciseController'),
(4,'2024-07-30 23:06:04','Updated data: User: string01 logged in','','Login','UserController'),
(4,'2024-07-30 23:06:09','Updated data: Admin viewed all users','','GetAllUsers','UserController'),
(4,'2024-07-31 02:57:59','Updated data: User: string01 logged in','','Login','UserController'),
(4,'2024-07-31 02:58:06','Updated data: Admin viewed all users','','GetAllUsers','UserController'),
(4,'2024-07-31 03:01:29','Updated data: Admin viewed all users','','GetAllUsers','UserController'),
(4,'2024-07-31 03:05:02','Updated data: User: pswB1 updated','','UpdateUser','UserController'),
(4,'2024-07-31 03:05:21','Updated data: User: pswB1 logged in','','Login','UserController'),
(4,'2024-07-31 03:05:43','Updated data: User: pswB1 failed to login','','Login','UserController'),
(4,'2024-07-31 03:05:47','Updated data: User: pswB1 failed to login','','Login','UserController'),
(4,'2024-07-31 03:05:52','Updated data: User: pswB1 failed to login','','Login','UserController'),
(4,'2024-07-31 03:06:21','Updated data: User: string01 logged in','','Login','UserController'),
(4,'2024-07-31 03:06:25','Updated data: User: pswB1 unlocked','','UnlockUser','UserController'),
(4,'2024-07-31 03:06:45','Updated data: User: pswB updated','','UpdateUser','UserController'),
(4,'2024-07-31 03:06:55','Updated data: User: pswB logged in','','Login','UserController'),
(5,'2024-07-31 03:08:02','Deleted data: User: TT deleted','','DeleteUser','UserController'),
(3,'2024-07-31 03:09:17','Created a new data Course created: accounting3','','CreateCourse','CourseController'),
(3,'2024-07-31 03:11:39','Created a new data Assigned User ID: 7 to Course ID: 6','','AssignCourse','CourseController'),
(4,'2024-07-31 03:12:26','Updated data: User: instructor logged in','','Login','UserController'),
(4,'2024-07-31 03:12:30','Updated data: User ID: 7 viewed their courses.','','ViewMyCourses','CourseController'),
(4,'2024-07-31 03:12:40','Updated data: Instructor ID: 7 viewed students in course ID: 4','','GetStudentsInCourse','CourseController'),
(3,'2024-07-31 03:13:54','Created a new data Exercise created: LCCI','','CreateExercise','ExerciseController'),
(4,'2024-07-31 03:17:37','Updated data: User: instructor logged in','','Login','UserController'),
(3,'2024-07-31 03:20:03','Created a new data Assigned Exercise ID: 5 assigned to Student ID: 3 in Course ID: 3','','AssignExercise','ExerciseController'),
(3,'2024-07-31 03:21:55','Created a new data Assigned Exercise ID: 1 assigned to Student ID: 3 in Course ID: 1','','AssignExercise','ExerciseController'),
(3,'2024-07-31 03:24:33','Created a new data Exercise created: LCCI12','','CreateExercise','ExerciseController'),
(4,'2024-07-31 03:26:40','Updated data: User: student logged in','','Login','UserController'),
(4,'2024-07-31 03:26:59','Updated data: User: student logged in','','Login','UserController'),
(4,'2024-07-31 03:28:05','Updated data: User ID: 3 viewed their exercises.','','ViewMyExercises','ExerciseController'),
(4,'2024-07-31 03:28:18','Updated data: User: instructor logged in','','Login','UserController'),
(4,'2024-07-31 03:30:38','Updated data: User: string01 logged in','','Login','UserController'),
(3,'2024-07-31 03:30:42','Created a new data Assigned User ID: 7 to Course ID: 1','','AssignCourse','CourseController'),
(3,'2024-07-31 03:30:48','Created a new data Assigned User ID: 7 to Course ID: 3','','AssignCourse','CourseController'),
(3,'2024-07-31 03:33:12','Created a new data Assigned User ID: 7 to Course ID: 5','','AssignCourse','CourseController'),
(3,'2024-07-31 03:34:28','Created a new data Assigned User ID: 3 to Course ID: 2','','AssignCourse','CourseController'),
(3,'2024-07-31 03:34:33','Created a new data Assigned User ID: 3 to Course ID: 6','','AssignCourse','CourseController'),
(3,'2024-07-31 03:35:46','Created a new data Assigned User ID: 5 to Course ID: 6','','AssignCourse','CourseController'),
(3,'2024-07-31 03:36:16','Created a new data Assigned User ID: 5 to Course ID: 1','','AssignCourse','CourseController'),
(4,'2024-07-31 03:36:27','Updated data: User: instructor logged in','','Login','UserController'),
(3,'2024-07-31 03:37:18','Created a new data Assigned Exercise ID: 1 assigned to Student ID: 5 in Course ID: 1','','AssignExercise','ExerciseController'),
(3,'2024-07-31 03:39:00','Created a new data Assigned Exercise ID: 4 assigned to Student ID: 5 in Course ID: 4','','AssignExercise','ExerciseController'),
(4,'2024-07-31 03:43:32','Updated data: Instructor ID: 7 viewed students in course ID: 2','','GetStudentsInCourse','CourseController'),
(3,'2024-07-31 03:44:41','Created a new data Assigned Exercise ID: 2 assigned to Student ID: 3 in Course ID: 2','','AssignExercise','ExerciseController'),
(3,'2024-07-31 03:45:20','Created a new data Exercise ID: 2 marked for Student ID: r0NNMM//vrPVK3KtS0OPiQ==','','MarkExercise','ExerciseController'),
(3,'2024-07-31 03:45:36','Created a new data Exercise ID: 3 marked for Student ID: r0NNMM//vrPVK3KtS0OPiQ==','','MarkExercise','ExerciseController'),
(3,'2024-07-31 03:45:48','Created a new data Exercise ID: 4 marked for Student ID: r0NNMM//vrPVK3KtS0OPiQ==','','MarkExercise','ExerciseController'),
(4,'2024-07-31 03:46:08','Updated data: User ID: 7 viewed their courses.','','ViewMyCourses','CourseController'),
(4,'2024-07-31 03:48:00','Updated data: Instructor ID: 7 viewed students in course ID: 2','','GetStudentsInCourse','CourseController'),
(4,'2024-07-31 03:48:17','Updated data: Instructor ID: 7 viewed students in course ID: 3','','GetStudentsInCourse','CourseController'),
(4,'2024-07-31 03:48:27','Updated data: User: string01 logged in','','Login','UserController'),
(4,'2024-07-31 03:48:46','Updated data: Admin viewed all users','','GetAllUsers','UserController'),
(3,'2024-07-31 03:49:22','Created a new data Assigned User ID: 6 to Course ID: 1','','AssignCourse','CourseController'),
(3,'2024-07-31 03:49:26','Created a new data Assigned User ID: 6 to Course ID: 2','','AssignCourse','CourseController'),
(3,'2024-07-31 03:49:30','Created a new data Assigned User ID: 6 to Course ID: 3','','AssignCourse','CourseController'),
(4,'2024-07-31 03:49:37','Updated data: User: instructor logged in','','Login','UserController'),
(4,'2024-07-31 03:49:42','Updated data: Instructor ID: 7 viewed students in course ID: 3','','GetStudentsInCourse','CourseController'),
(4,'2024-07-31 03:50:16','Updated data: Instructor ID: 7 viewed students in course ID: 3','','GetStudentsInCourse','CourseController'),
(4,'2024-07-31 03:50:57','Updated data: Instructor banned student ID: jqwB92rLfbPkOMyGFJe0IA== from course ID: 3','','BanStudentFromCourse','CourseController'),
(4,'2024-07-31 03:51:21','Updated data: Instructor ID: 7 viewed students in course ID: 3','','GetStudentsInCourse','CourseController'),
(4,'2024-07-31 03:52:16','Updated data: User: student logged in','','Login','UserController'),
(4,'2024-07-31 03:52:21','Updated data: User ID: 3 viewed their exercises.','','ViewMyExercises','ExerciseController');

UNLOCK TABLES;

/*Table structure for table `exercise_assignments` */

DROP TABLE IF EXISTS `exercise_assignments`;

CREATE TABLE `exercise_assignments` (
  `ExerciseAssignmentID` int NOT NULL AUTO_INCREMENT,
  `ExerciseID` int NOT NULL,
  `StudentID` int NOT NULL,
  `AssignedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CourseID` int DEFAULT NULL,
  PRIMARY KEY (`ExerciseAssignmentID`),
  KEY `ExerciseID` (`ExerciseID`),
  KEY `StudentID` (`StudentID`),
  KEY `CourseID` (`CourseID`),
  CONSTRAINT `exercise_assignments_ibfk_1` FOREIGN KEY (`ExerciseID`) REFERENCES `exercises` (`ExerciseID`),
  CONSTRAINT `exercise_assignments_ibfk_2` FOREIGN KEY (`StudentID`) REFERENCES `users` (`UserID`),
  CONSTRAINT `exercise_assignments_ibfk_3` FOREIGN KEY (`CourseID`) REFERENCES `courses` (`CourseID`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `exercise_assignments` */

LOCK TABLES `exercise_assignments` WRITE;

insert  into `exercise_assignments`(`ExerciseAssignmentID`,`ExerciseID`,`StudentID`,`AssignedAt`,`CourseID`) values 
(1,1,3,'2024-07-31 01:18:26',1),
(2,3,3,'2024-07-31 01:18:42',3),
(3,4,5,'2024-07-31 01:19:22',4),
(4,5,3,'0001-01-01 00:00:00',3),
(6,1,5,'0001-01-01 00:00:00',1),
(8,2,3,'0001-01-01 00:00:00',2);

UNLOCK TABLES;

/*Table structure for table `exercises` */

DROP TABLE IF EXISTS `exercises`;

CREATE TABLE `exercises` (
  `ExerciseID` int NOT NULL AUTO_INCREMENT,
  `CourseID` int NOT NULL,
  `Title` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Description` text COLLATE utf8mb4_unicode_ci,
  `CreatedBy` int NOT NULL,
  `CreatedAt` datetime NOT NULL,
  PRIMARY KEY (`ExerciseID`),
  KEY `CourseID` (`CourseID`),
  KEY `CreatedBy` (`CreatedBy`),
  CONSTRAINT `exercises_ibfk_1` FOREIGN KEY (`CourseID`) REFERENCES `courses` (`CourseID`),
  CONSTRAINT `exercises_ibfk_2` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `exercises` */

LOCK TABLES `exercises` WRITE;

insert  into `exercises`(`ExerciseID`,`CourseID`,`Title`,`Description`,`CreatedBy`,`CreatedAt`) values 
(1,1,'Math Exercise 1','Solve the equations',2,'2024-07-24 14:51:48'),
(2,2,'Science Exercise 1','Explain the laws of physics',2,'2024-07-24 14:51:48'),
(3,3,'introduction of programming','Hello World',7,'2024-07-29 15:59:21'),
(4,4,'introduction ','Hello World',7,'2024-07-29 15:59:37'),
(5,3,'ssss','sssss',7,'2024-07-30 19:39:26'),
(6,6,'LCCI','Bookeeping',7,'2024-07-31 03:13:54'),
(7,6,'LCCI12','Bookeeping22',7,'2024-07-31 03:24:33');

UNLOCK TABLES;

/*Table structure for table `grades` */

DROP TABLE IF EXISTS `grades`;

CREATE TABLE `grades` (
  `GradeID` int NOT NULL AUTO_INCREMENT,
  `ExerciseID` int NOT NULL,
  `StudentID` int NOT NULL,
  `InstructorID` int NOT NULL,
  `GradeValue` int NOT NULL,
  `GradedAt` datetime NOT NULL,
  PRIMARY KEY (`GradeID`),
  KEY `ExerciseID` (`ExerciseID`),
  KEY `idx_student_exercise` (`StudentID`,`ExerciseID`),
  KEY `idx_instructor_exercise` (`InstructorID`,`ExerciseID`),
  CONSTRAINT `grades_ibfk_1` FOREIGN KEY (`ExerciseID`) REFERENCES `exercises` (`ExerciseID`),
  CONSTRAINT `grades_ibfk_2` FOREIGN KEY (`StudentID`) REFERENCES `users` (`UserID`),
  CONSTRAINT `grades_ibfk_3` FOREIGN KEY (`InstructorID`) REFERENCES `users` (`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `grades` */

LOCK TABLES `grades` WRITE;

insert  into `grades`(`GradeID`,`ExerciseID`,`StudentID`,`InstructorID`,`GradeValue`,`GradedAt`) values 
(1,1,1,2,85,'2024-07-24 14:52:13'),
(2,2,1,2,90,'2024-07-24 14:52:13'),
(3,4,3,7,70,'2024-07-30 20:17:22'),
(4,2,3,7,100,'2024-07-31 03:45:19'),
(5,3,3,7,100,'2024-07-31 03:45:36'),
(6,4,3,7,50,'2024-07-31 03:45:48');

UNLOCK TABLES;

/*Table structure for table `role_permissions` */

DROP TABLE IF EXISTS `role_permissions`;

CREATE TABLE `role_permissions` (
  `RolePermissionID` int NOT NULL AUTO_INCREMENT,
  `RoleID` int NOT NULL,
  `Permission` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`RolePermissionID`),
  KEY `RoleID` (`RoleID`),
  CONSTRAINT `role_permissions_ibfk_1` FOREIGN KEY (`RoleID`) REFERENCES `roles` (`RoleID`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `role_permissions` */

LOCK TABLES `role_permissions` WRITE;

insert  into `role_permissions`(`RolePermissionID`,`RoleID`,`Permission`) values 
(1,1,'GetAllUsers'),
(5,1,'CreateUser'),
(6,1,'UpdateUser'),
(7,1,'CreateCourse'),
(8,2,'ViewMyCourses'),
(9,3,'ViewMyCourses'),
(10,1,'UnlockUser'),
(11,1,'DeleteUser'),
(12,1,'AssignCourse'),
(13,2,'CreateExercise'),
(14,2,'MarkExercise'),
(15,2,'ViewMyExercises'),
(16,3,'ViewMyExercisesWithGrades'),
(17,2,'GetStudentsInCourse'),
(18,2,'AssignExercise'),
(19,2,'BanStudentFromCourse');

UNLOCK TABLES;

/*Table structure for table `roles` */

DROP TABLE IF EXISTS `roles`;

CREATE TABLE `roles` (
  `RoleID` int NOT NULL AUTO_INCREMENT,
  `RoleName` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`RoleID`),
  UNIQUE KEY `RoleName` (`RoleName`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `roles` */

LOCK TABLES `roles` WRITE;

insert  into `roles`(`RoleID`,`RoleName`) values 
(1,'Admin'),
(2,'Instructor'),
(3,'Student');

UNLOCK TABLES;

/*Table structure for table `users` */

DROP TABLE IF EXISTS `users`;

CREATE TABLE `users` (
  `UserID` int NOT NULL AUTO_INCREMENT,
  `UserName` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `FirstName` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `LastName` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Email` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Password` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `salt` varchar(500) COLLATE utf8mb4_unicode_ci NOT NULL,
  `login_fail_count` int NOT NULL DEFAULT '0',
  `is_lock` tinyint(1) NOT NULL DEFAULT '0',
  `RoleID` int NOT NULL,
  `CreatedAt` datetime NOT NULL,
  PRIMARY KEY (`UserID`),
  UNIQUE KEY `UserName` (`UserName`),
  UNIQUE KEY `Email` (`Email`),
  UNIQUE KEY `idx_email` (`Email`),
  UNIQUE KEY `idx_username` (`UserName`),
  KEY `RoleID` (`RoleID`),
  CONSTRAINT `users_ibfk_1` FOREIGN KEY (`RoleID`) REFERENCES `roles` (`RoleID`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `users` */

LOCK TABLES `users` WRITE;

insert  into `users`(`UserID`,`UserName`,`FirstName`,`LastName`,`Email`,`Password`,`salt`,`login_fail_count`,`is_lock`,`RoleID`,`CreatedAt`) values 
(1,'string01','string','string','string001@gmail.com','1fEGVxFWOgE6gQEKWMjwSNK7BClJ7z0L','is/bWPaN1zZuWDyAVmTvCMFl/Kl8fZIE',0,0,1,'2024-07-26 11:14:41'),
(2,'kayy','puu','tuu','puutuu@gmail.com','cQakv8yQ10tNGqBaljVQ0hyoFloAvSaZ','q5UQRGlClulMXZ0CkmtZbLjxF1zGqg15',0,0,1,'2024-07-29 11:16:14'),
(3,'student','bb','bbb','student03@gmail.com','ppjA4iDyOQRR3Fdh46BqSR18RG0HOnQZ','1HZQCPCyg/Pen6q4NEjIWIIB5yUxzo1m',0,0,3,'2024-07-29 11:17:01'),
(5,'student03','bbb','vv','student003@gmail.com','OLyS/VsEw5V5YsEz6WeUxzbFvrxKFAN+','v4155IKN0B+75FkVBXFK1lCNAXtoTwUL',0,0,3,'2024-07-29 11:17:37'),
(6,'student003','q','bbb','student3@gmail.com','JkqghyPaAe15TvXOnQYawu1vLC+QE1DN','FhcA3jYOJKv8T3s6AdYOdUtZSM85zBin',0,0,3,'2024-07-29 11:17:52'),
(7,'instructor','bb','bb','instructor2@gmail.com','p7S9ts5tl76Vc9sGBIiZ6wlYK4EbPZY9','r2yGed2HHsypzOIleKZtjbhSFA+PvlsK',0,0,2,'2024-07-29 11:18:58'),
(8,'instructor02','bb','bb','instructor02@gmail.com','etmKvIeuK85Pr/W24r1w4AizGpUTlO1U','tOpUAauFStQUcoOVRi+2TuyajYpMU4ca',0,0,2,'2024-07-29 11:19:13'),
(9,'instructor002','bb','bb','instructor002@gmail.com','0WS6kQ+6I0+ZsJvNu5/VxopUlUTY5p3a','f4hWSUliK0AdAF1htHKtrd4YY81cLz6R',0,0,2,'2024-07-29 11:19:19'),
(10,'admin','dd','dd','admin1@gmail.com','iPUcbi7dwhQncQVLDiV0XHj4nBahjfEr','BmyUAUwRkHMPOixa4k+Og9/vuBVuW6ie',0,0,1,'2024-07-29 11:20:04'),
(11,'pswB','bb','bb','b1@gmail.com','nTuIY1mUSYELjRf9Abo3WInhBWDUi6Q+','+HoroXqs5SFel1LFBMhf0Un9kTD82sBI',0,0,1,'2024-07-30 14:19:36');

UNLOCK TABLES;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
