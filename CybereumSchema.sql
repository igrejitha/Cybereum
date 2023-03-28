CREATE DATABASE  IF NOT EXISTS `cybereum` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `cybereum`;
-- MySQL dump 10.13  Distrib 8.0.29, for Win64 (x86_64)
--
-- Host: 172.16.3.162    Database: cybereum
-- ------------------------------------------------------
-- Server version	5.6.21-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `tbl_milestone`
--

DROP TABLE IF EXISTS `tbl_milestone`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tbl_milestone` (
  `milestoneid` int(11) NOT NULL AUTO_INCREMENT,
  `milestonename` varchar(500) NOT NULL,
  `isactive` int(11) NOT NULL,
  `startdate` datetime NOT NULL,
  `enddate` datetime NOT NULL,
  `statusid` int(11) NOT NULL,
  `createdby` int(11) NOT NULL,
  `createddate` datetime NOT NULL,
  `modifiedby` int(11) DEFAULT NULL,
  `modifieddate` datetime DEFAULT NULL,
  `projectid` int(11) NOT NULL,
  `priority` int(11) NOT NULL,
  `tasktypeid` int(11) DEFAULT NULL,
  `assignedto` int(11) NOT NULL,
  PRIMARY KEY (`milestoneid`),
  KEY `FK_milestoneassignedto` (`assignedto`),
  KEY `FK_milestoneprojectid` (`projectid`),
  KEY `FK_milestonetasktype` (`tasktypeid`),
  CONSTRAINT `FK_milestoneassignedto` FOREIGN KEY (`assignedto`) REFERENCES `tbl_user` (`userid`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_milestoneprojectid` FOREIGN KEY (`projectid`) REFERENCES `tbl_project` (`projectid`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_milestonetasktype` FOREIGN KEY (`tasktypeid`) REFERENCES `tbl_tasktype` (`tasktypeid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tbl_milestone`
--

LOCK TABLES `tbl_milestone` WRITE;
/*!40000 ALTER TABLE `tbl_milestone` DISABLE KEYS */;
/*!40000 ALTER TABLE `tbl_milestone` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tbl_project`
--

DROP TABLE IF EXISTS `tbl_project`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tbl_project` (
  `projectid` int(11) NOT NULL AUTO_INCREMENT,
  `projectname` varchar(1000) NOT NULL,
  `startdate` datetime NOT NULL,
  `enddate` datetime NOT NULL,
  `createdon` datetime NOT NULL,
  `modifiedon` datetime DEFAULT NULL,
  `createdby` int(11) NOT NULL,
  `modifiedby` int(11) DEFAULT NULL,
  `projectcost` int(20) DEFAULT NULL,
  `noofresource` int(11) DEFAULT NULL,
  `isactive` int(11) NOT NULL,
  PRIMARY KEY (`projectid`),
  KEY `FK_createdby_idx` (`createdby`),
  KEY `FK_updatedby_idx` (`modifiedby`),
  CONSTRAINT `FK_createdby` FOREIGN KEY (`createdby`) REFERENCES `tbl_user` (`userid`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_updatedby` FOREIGN KEY (`modifiedby`) REFERENCES `tbl_user` (`userid`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tbl_project`
--

LOCK TABLES `tbl_project` WRITE;
/*!40000 ALTER TABLE `tbl_project` DISABLE KEYS */;
INSERT INTO `tbl_project` VALUES (3,'Project1','2023-03-27 00:00:00','2023-03-28 00:00:00','2023-03-27 10:37:20','2023-03-27 13:51:52',2,2,0,5,1),(4,'Project2','2023-03-27 00:00:00','2023-03-24 00:00:00','2023-03-27 11:27:44',NULL,2,NULL,NULL,NULL,1),(5,'Project3','2023-03-27 00:00:00','2023-03-27 00:00:00','2023-03-27 12:16:56',NULL,2,NULL,0,0,1),(6,'Project4','2023-03-27 00:00:00','2023-03-27 00:00:00','2023-03-27 15:17:21',NULL,2,NULL,0,0,1);
/*!40000 ALTER TABLE `tbl_project` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tbl_projectdet`
--

DROP TABLE IF EXISTS `tbl_projectdet`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tbl_projectdet` (
  `projectdetid` int(11) NOT NULL AUTO_INCREMENT,
  `projectid` int(11) NOT NULL,
  `userid` int(11) NOT NULL,
  `createdby` int(11) DEFAULT NULL,
  `createdon` datetime DEFAULT NULL,
  `modifiedby` int(11) DEFAULT NULL,
  `modifiedon` datetime DEFAULT NULL,
  PRIMARY KEY (`projectdetid`),
  KEY `FK_projectid_idx` (`projectid`),
  KEY `FK_userid_idx` (`userid`),
  KEY `FK_createdby_idx` (`createdby`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tbl_projectdet`
--

LOCK TABLES `tbl_projectdet` WRITE;
/*!40000 ALTER TABLE `tbl_projectdet` DISABLE KEYS */;
/*!40000 ALTER TABLE `tbl_projectdet` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tbl_status`
--

DROP TABLE IF EXISTS `tbl_status`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tbl_status` (
  `statusid` int(11) NOT NULL AUTO_INCREMENT,
  `statusname` varchar(500) NOT NULL,
  `isactive` int(11) NOT NULL,
  `createdby` int(11) NOT NULL,
  `createdon` datetime NOT NULL,
  `modifiedby` int(11) DEFAULT NULL,
  `modifiedon` datetime DEFAULT NULL,
  PRIMARY KEY (`statusid`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tbl_status`
--

LOCK TABLES `tbl_status` WRITE;
/*!40000 ALTER TABLE `tbl_status` DISABLE KEYS */;
INSERT INTO `tbl_status` VALUES (1,'Open',1,1,'2023-03-25 00:00:00',NULL,NULL),(2,'Completed',1,1,'2023-03-25 00:00:00',NULL,NULL),(3,'In progress',1,1,'2023-03-25 00:00:00',NULL,NULL);
/*!40000 ALTER TABLE `tbl_status` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tbl_task`
--

DROP TABLE IF EXISTS `tbl_task`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tbl_task` (
  `taskid` int(11) NOT NULL AUTO_INCREMENT,
  `taskname` varchar(500) NOT NULL,
  `isactive` int(11) NOT NULL,
  `startdate` datetime NOT NULL,
  `enddate` datetime NOT NULL,
  `statusid` int(11) NOT NULL,
  `createdby` int(11) NOT NULL,
  `createddate` datetime NOT NULL,
  `modifiedby` int(11) DEFAULT NULL,
  `modifieddate` datetime DEFAULT NULL,
  `projectid` int(11) NOT NULL,
  `priority` int(11) NOT NULL,
  `tasktypeid` int(11) DEFAULT NULL,
  `assignedto` int(11) NOT NULL,
  PRIMARY KEY (`taskid`),
  KEY `FK_projectid_idx` (`projectid`),
  KEY `FK_createdby_idx` (`createdby`),
  KEY `FK_modifiedby_idx` (`modifiedby`),
  KEY `FK_taskstatus_idx` (`statusid`),
  KEY `tasktypeid` (`tasktypeid`),
  KEY `FK_assignedto_idx` (`assignedto`),
  CONSTRAINT `FK_assignedto` FOREIGN KEY (`assignedto`) REFERENCES `tbl_user` (`userid`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_projectid` FOREIGN KEY (`projectid`) REFERENCES `tbl_project` (`projectid`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `tbl_task_ibfk_1` FOREIGN KEY (`tasktypeid`) REFERENCES `tbl_tasktype` (`tasktypeid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tbl_task`
--

LOCK TABLES `tbl_task` WRITE;
/*!40000 ALTER TABLE `tbl_task` DISABLE KEYS */;
/*!40000 ALTER TABLE `tbl_task` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tbl_tasktype`
--

DROP TABLE IF EXISTS `tbl_tasktype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tbl_tasktype` (
  `tasktypeid` int(11) NOT NULL AUTO_INCREMENT,
  `tasktypename` varchar(250) NOT NULL,
  `isactive` int(11) NOT NULL,
  `createdby` int(11) NOT NULL,
  `createdon` datetime NOT NULL,
  `modifiedby` int(11) DEFAULT NULL,
  `modifiedon` datetime DEFAULT NULL,
  PRIMARY KEY (`tasktypeid`),
  KEY `FK_createdby_idx` (`createdby`),
  KEY `FK_modifiedby_idx` (`modifiedby`),
  CONSTRAINT `FK_modifiedby` FOREIGN KEY (`modifiedby`) REFERENCES `tbl_user` (`userid`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tbl_tasktype`
--

LOCK TABLES `tbl_tasktype` WRITE;
/*!40000 ALTER TABLE `tbl_tasktype` DISABLE KEYS */;
INSERT INTO `tbl_tasktype` VALUES (1,'Bug',1,1,'2023-03-25 00:00:00',NULL,NULL),(2,'Task',1,1,'2023-03-25 00:00:00',NULL,NULL);
/*!40000 ALTER TABLE `tbl_tasktype` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tbl_user`
--

DROP TABLE IF EXISTS `tbl_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tbl_user` (
  `userid` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(100) DEFAULT NULL,
  `password` varchar(5000) NOT NULL,
  `roleid` int(11) NOT NULL,
  `firstname` varchar(100) DEFAULT NULL,
  `lastname` varchar(100) DEFAULT NULL,
  `isactive` int(11) DEFAULT NULL,
  `emailid` varchar(500) DEFAULT NULL,
  `organization` varchar(500) DEFAULT NULL,
  `createddate` datetime DEFAULT NULL,
  `emailverification` bit(1) DEFAULT NULL,
  `otp` varchar(4) DEFAULT NULL,
  `activationcode` varchar(5000) DEFAULT NULL,
  `pmuserid` int(11) DEFAULT NULL,
  PRIMARY KEY (`userid`),
  KEY `FK_userrole_idx` (`roleid`),
  KEY `fk_pmuser` (`pmuserid`),
  CONSTRAINT `FK_userrole` FOREIGN KEY (`roleid`) REFERENCES `tbl_userrole` (`roleid`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_pmuser` FOREIGN KEY (`pmuserid`) REFERENCES `tbl_user` (`userid`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tbl_user`
--

LOCK TABLES `tbl_user` WRITE;
/*!40000 ALTER TABLE `tbl_user` DISABLE KEYS */;
INSERT INTO `tbl_user` VALUES (1,'admin','NRmacANXZLxcP3FyLn0u+fQEfntDUOQiRuhYv8lVjYc=',1,'admin','admin',1,'admin@infognana.com','Infognana','2023-03-16 00:00:00',NULL,NULL,NULL,NULL),(2,NULL,'PAQUWcts3xlLYobbK5EcNU6dbT8wL9Il88qBvqw/hAU=',2,'Rejitha','C',1,'rejitha.c@infognana.com','Infognana','2023-03-16 00:00:00',_binary '','1255','746f072e-ad96-4eaa-bd97-a12fc22e0b57',NULL),(3,'','PAQUWcts3xlLYobbK5EcNU6dbT8wL9Il88qBvqw/hAU=',3,'Rejitha','Chandran',1,'rejibca@gmail.com','Infognana','2023-03-21 15:08:06',_binary '','2901','746f072e-ad96-4eaa-bd98-a12fc22e0b57',2),(4,'','CIule42OnRZD2if/qkzucXhMKbu0CdcclfLWlua9YAY=',2,'Ujithkumar','U',1,'ujithkumar.u@infognana.com','Infognana','2023-03-24 09:41:17',_binary '','0307','e1473387-0bbd-42ff-9434-fa55774d8002',NULL),(5,'','dNADtSZ4FjNjI1vcCI9bVkUncAwKAFd6QL+yVAyzLfg=',3,'Ananth','Natarajan',0,'ananth.natarajan@cybereum.io','Cybereum','2023-03-24 09:46:22',_binary '\0','0500','bd44692f-7740-4c6b-ad52-1bff7be6ee21',2);
/*!40000 ALTER TABLE `tbl_user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tbl_userrole`
--

DROP TABLE IF EXISTS `tbl_userrole`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tbl_userrole` (
  `roleid` int(11) NOT NULL AUTO_INCREMENT,
  `rolename` varchar(50) NOT NULL,
  `isactive` int(11) NOT NULL,
  PRIMARY KEY (`roleid`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tbl_userrole`
--

LOCK TABLES `tbl_userrole` WRITE;
/*!40000 ALTER TABLE `tbl_userrole` DISABLE KEYS */;
INSERT INTO `tbl_userrole` VALUES (1,'Admin',1),(2,'Project Manager',1),(3,'User',1);
/*!40000 ALTER TABLE `tbl_userrole` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'cybereum'
--

--
-- Dumping routines for database 'cybereum'
--
/*!50003 DROP PROCEDURE IF EXISTS `sp_FetchApprovedUsers` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`igs`@`%` PROCEDURE `sp_FetchApprovedUsers`( in P_Pageno INT, 
in P_pagesize INT, in P_SortColumn varchar(100), in P_SortOrder varchar(100))
BEGIN

SET @countsql:=CONCAT("SELECT count(userid) INTO @RecTotalCount FROM tbl_user A",
					" WHERE A.isactive=1 and A.roleid<>1;");  	
    
    PREPARE dynamic_statement FROM @countsql;
    -- select @countsql;
	EXECUTE dynamic_statement;
	DEALLOCATE PREPARE dynamic_statement;
     
    
  SET @sql:=CONCAT("SELECT A.Userid,A.firstname,A.lastname,A.emailid,A.`organization` organization,A.roleid,A.isactive
    ,B.rolename as rolename,",@RecTotalCount," as TotalRecordCount
    FROM tbl_user A INNER JOIN tbl_userrole B on A.roleid=B.roleid 
    where A.isactive=1 and A.roleid<>1 ");      	
    
	SET @sql:=Concat(@sql, " order by ",P_SortColumn," ",P_SortOrder," limit ",P_pagesize," OFFSET ",P_Pageno,"");
    
	PREPARE dynamic_statement FROM @sql; 
    -- select @sql;
	EXECUTE dynamic_statement;
	DEALLOCATE PREPARE dynamic_statement;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_FetchLoginDetails` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`igs`@`%` PROCEDURE `sp_FetchLoginDetails`(in P_email varchar(100), in P_password varchar(100))
BEGIN	
	IF EXISTS(Select 1 from tbl_user where emailid = P_email and password = P_password and isactive=1) THEN    
		Select userid, roleid, emailid from tbl_user where emailid = P_email and password = P_password and isactive=1;		
	else
		select -1 as userid, 0  as roleid,'-' emailid;
    end if;	
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_FetchPendingUsers` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`igs`@`%` PROCEDURE `sp_FetchPendingUsers`( in P_Pageno INT, 
in P_pagesize INT, in P_SortColumn varchar(100), in P_SortOrder varchar(100))
BEGIN

SET @countsql:=CONCAT("SELECT count(userid) INTO @RecTotalCount FROM tbl_user A",
					" WHERE A.isactive=0 and A.emailverification=true;");  	
    
    PREPARE dynamic_statement FROM @countsql;
    -- select @countsql;
	EXECUTE dynamic_statement;
	DEALLOCATE PREPARE dynamic_statement;
     
    
  SET @sql:=CONCAT("SELECT A.Userid,A.firstname,A.lastname,A.emailid,A.`organization` organization,A.roleid,A.isactive
    ,B.rolename as rolename,",@RecTotalCount," as TotalRecordCount
    FROM tbl_user A INNER JOIN tbl_userrole B on A.roleid=B.roleid 
    where A.isactive=0 and A.emailverification=true ");      	
    
	SET @sql:=Concat(@sql, " order by ",P_SortColumn," ",P_SortOrder," limit ",P_pagesize," OFFSET ",P_Pageno,"");
    
	PREPARE dynamic_statement FROM @sql; 
    -- select @sql;
	EXECUTE dynamic_statement;
	DEALLOCATE PREPARE dynamic_statement;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_FetchProjectExists` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`igs`@`%` PROCEDURE `sp_FetchProjectExists`(in P_projectname varchar(100))
BEGIN	
	IF EXISTS(Select 1 from tbl_project where projectname = P_projectname and isactive=1) THEN    
		Select projectid from tbl_project where projectname = P_projectname and isactive=1;		
	else
		select -1 as projectid;
    end if;	
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_FetchProjects` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`igs`@`%` PROCEDURE `sp_FetchProjects`( in P_PMid INT,in P_Roleid INT,in P_Pageno INT, 
in P_pagesize INT, in P_SortColumn varchar(100), in P_SortOrder varchar(100))
BEGIN

SET @countsql:=CONCAT("SELECT count(projectid) INTO @RecTotalCount FROM tbl_project A",
					" WHERE A.isactive=1"); 
                    
	if(P_Roleid = 2 ) then		
        SET @countsql:= Concat(@countsql,	" and A.createdby=",P_PMid);
	end if;    
    
    PREPARE dynamic_statement FROM @countsql;
    -- select @countsql;
	EXECUTE dynamic_statement;
	DEALLOCATE PREPARE dynamic_statement;
     
    
  SET @sql:=CONCAT("SELECT A.projectid,A.projectname,A.startdate,A.enddate,A.projectcost,A.noofresource
	,A.isactive,A.createdby,A.createdon,A.modifiedby,A.modifiedon
    ,concat(B.firstname,' ',B.Lastname) as createdusername,concat(C.firstname,' ',C.Lastname) as modifiedusername,",@RecTotalCount," as TotalRecordCount
    FROM tbl_project A
    INNER JOIN tbl_user B on A.createdby=B.userid 
    LEFT JOIN tbl_user C on A.modifiedby=C.userid 
    where A.isactive=1 ");      	
    
    if(P_Roleid = 2 ) then		
        SET @sql:= Concat(@sql,	" and A.createdby=",P_PMid);
	end if;
    
	SET @sql:=Concat(@sql, " order by ",P_SortColumn," ",P_SortOrder," limit ",P_pagesize," OFFSET ",P_Pageno,"");
    
	PREPARE dynamic_statement FROM @sql; 
    -- select P_Roleid;
    -- select @sql;
	EXECUTE dynamic_statement;
	DEALLOCATE PREPARE dynamic_statement;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_FetchTaskExists` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`igs`@`%` PROCEDURE `sp_FetchTaskExists`(in P_taskname varchar(100))
BEGIN	
	IF EXISTS(Select 1 from tbl_task where taskname = P_taskname and isactive=1) THEN    
		Select taskid from tbl_task where taskname = P_taskname and isactive=1;		
	else
		select -1 as taskid;
    end if;	
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_FetchTasks` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`igs`@`%` PROCEDURE `sp_FetchTasks`( in P_PMid INT,in P_Roleid INT,in P_Projectid INT,in P_Pageno INT, 
in P_pagesize INT, in P_SortColumn varchar(100), in P_SortOrder varchar(100))
BEGIN

SET @countsql:=CONCAT("SELECT count(taskid) INTO @RecTotalCount FROM tbl_task A",
					" WHERE A.isactive=1 and A.projectid=",P_Projectid); 
                    
	if(P_Roleid = 3 ) then		
        SET @countsql:= Concat(@countsql,	" and A.assignedto=",P_PMid);
	end if;    
    
    PREPARE dynamic_statement FROM @countsql;
    -- select @countsql;
	EXECUTE dynamic_statement;
	DEALLOCATE PREPARE dynamic_statement;
     
    
  SET @sql:=CONCAT("SELECT A.taskid,A.taskname,A.Projectid,D.projectname,A.startdate,A.enddate
	,A.isactive,A.createdby,A.createddate,A.modifiedby,A.modifieddate
    ,A.assignedto,concat(E.firstname,' ',E.Lastname) as assignedusername
    ,concat(B.firstname,' ',B.Lastname) as createdusername,concat(C.firstname,' ',C.Lastname) as modifiedusername,",@RecTotalCount," as TotalRecordCount
    FROM tbl_task A
    INNER JOIN tbl_project D on A.projectid=D.projectid
    INNER JOIN tbl_user B on A.createdby=B.userid 
    LEFT JOIN tbl_user C on A.modifiedby=C.userid 
    INNER JOIN tbl_user E on A.assignedto=E.userid 
    where A.isactive=1 and A.projectid=",P_Projectid);      	
    
    if(P_Roleid = 3 ) then		
        SET @sql:= Concat(@sql,	" and A.assignedto=",P_PMid);
	end if;
    
	SET @sql:=Concat(@sql, " order by ",P_SortColumn," ",P_SortOrder," limit ",P_pagesize," OFFSET ",P_Pageno,"");
    
	PREPARE dynamic_statement FROM @sql; 
    -- select P_Roleid;
    -- select @sql;
	EXECUTE dynamic_statement;
	DEALLOCATE PREPARE dynamic_statement;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_FetchUserExists` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`igs`@`%` PROCEDURE `sp_FetchUserExists`(in P_email varchar(100))
BEGIN	
	IF EXISTS(Select 1 from tbl_user where emailid = P_email and isactive<>2) THEN    
		Select userid from tbl_user where emailid = P_email and isactive<>2;		
	else
		select -1 as userid;
    end if;	
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_FetchUsers` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`igs`@`%` PROCEDURE `sp_FetchUsers`( in P_PMid INT,in P_Roleid INT,in P_Pageno INT, 
in P_pagesize INT, in P_SortColumn varchar(100), in P_SortOrder varchar(100))
BEGIN

SET @countsql:=CONCAT("SELECT count(userid) INTO @RecTotalCount FROM tbl_user A",
					" WHERE A.isactive=1"); 
                    
	if(P_Roleid = 2 ) then		
        SET @countsql:= Concat(@countsql,	" and A.roleid=3 and A.pmuserid=",P_PMid);
	end if;    
    
    PREPARE dynamic_statement FROM @countsql;
    -- select @countsql;
	EXECUTE dynamic_statement;
	DEALLOCATE PREPARE dynamic_statement;
     
    
  SET @sql:=CONCAT("SELECT A.Userid,A.firstname,A.lastname,A.emailid,A.`organization` organization,A.roleid,A.isactive
    ,B.rolename as rolename,",@RecTotalCount," as TotalRecordCount
    FROM tbl_user A INNER JOIN tbl_userrole B on A.roleid=B.roleid 
    where A.isactive=1 ");      	
    
    if(P_Roleid = 2 ) then		
        SET @sql:= Concat(@sql,	" and A.roleid=3 and A.pmuserid=",P_PMid);
	end if;
    
	SET @sql:=Concat(@sql, " order by ",P_SortColumn," ",P_SortOrder," limit ",P_pagesize," OFFSET ",P_Pageno,"");
    
	PREPARE dynamic_statement FROM @sql; 
    -- select P_Roleid;
    -- select @sql;
	EXECUTE dynamic_statement;
	DEALLOCATE PREPARE dynamic_statement;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2023-03-28 15:07:56
