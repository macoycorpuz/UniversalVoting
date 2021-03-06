USE [Borrowers_App]
GO
/****** Object:  UserDefinedFunction [dbo].[fnGetAssistantName]    Script Date: 3/6/2017 3:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fnGetAssistantName]
(
	@AssistantID			INT
)
RETURNS VARCHAR(MAX)
AS
BEGIN
	
	DECLARE @Return_Value					VARCHAR(MAX)

	SELECT 
		@Return_Value = (P.Given_Name + ' ' + P.Last_Name)

	FROM PERSON AS P
		INNER JOIN Assistant AS A
			ON A.Person_ID = P.Person_ID
	WHERE A.Assistant_ID = @AssistantID

	RETURN @Return_Value

END

GO
/****** Object:  UserDefinedFunction [dbo].[fnGetInstructrorName]    Script Date: 3/6/2017 3:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fnGetInstructrorName]
(
	@InstructorID			INT
)
RETURNS VARCHAR(MAX)
AS
BEGIN
	
	DECLARE @Return_Value					VARCHAR(MAX)

	SELECT 
		@Return_Value = (P.Given_Name + ' ' + P.Last_Name)

	FROM PERSON AS P
		INNER JOIN Instructor AS I
			ON I.Person_ID = P.Person_ID
	WHERE I.Instructor_ID = @InstructorID

	RETURN @Return_Value

END

GO
/****** Object:  StoredProcedure [dbo].[spAddCourseSection]    Script Date: 3/6/2017 3:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spAddCourseSection]
(
	@Course_Code					VARCHAR(20),
	@Section_Code					VARCHAR(10),
	@Term							INT,
	@Year							INT,
	@Instructor_Given_Name			VARCHAR(100),
	@Instructor_Last_Name			VARCHAR(100),
	@Assistant_Given_Name			VARCHAR(100),
	@Assistant_Last_Name			VARCHAR(100),
	@Username						VARCHAR(50),
	@Password						VARCHAR(50)						
)
AS
BEGIN

	DECLARE @Course_ID				INT
	DECLARE @Section_ID				INT
	DECLARE @Instructor_ID			INT
	DECLARE @Assistant_ID			INT
	DECLARE @Person_ID				INT

	--Check Course Code
	IF NOT EXISTS
	(
		SELECT C.Course_Code
		FROM Course AS C
		WHERE C.Course_Code = @Course_Code
	)
	BEGIN
		INSERT INTO Course (Course_Code)
		VALUES (@Course_Code)
	END
	--Get Course ID
	SELECT @Course_ID = C.Course_ID	
	FROM Course AS C	
	WHERE C.Course_Code = @Course_Code


	--Check Section Code
	IF NOT EXISTS
	(
		SELECT S.Section_Code
		FROM Section AS S
		WHERE S.Section_Code = @Section_Code
	)
	BEGIN
		INSERT INTO Section(Section_Code)
		VALUES (@Section_Code)
	END
	--Get Section ID
	SELECT @Section_ID = S.Section_ID	
	FROM Section AS S	
	WHERE S.Section_Code = @Section_Code


	--Check Instructor
	IF NOT EXISTS
	(
		SELECT
			P.Given_Name,
			P.Last_Name
		FROM Person AS P
		WHERE P.Given_Name = @Instructor_Given_Name
			AND P.Last_Name = @Instructor_Last_Name
	)
	BEGIN
		
		INSERT INTO Person (Given_Name, Last_Name)
		VALUES (@Instructor_Given_Name, @Instructor_Last_Name)
		INSERT INTO Instructor(Person_ID)
		SELECT P.Person_ID 
		FROM Person AS P 
		WHERE P.Given_Name = @Instructor_Given_Name 
			AND P.Last_Name = @Instructor_Last_Name
	END
	--Get Instructor ID
	SELECT @Instructor_ID = I.Instructor_ID	
		FROM Instructor AS I 
			INNER JOIN Person AS P
				ON I.Person_ID = P.Person_ID
		WHERE P.Given_Name = @Instructor_Given_Name
			AND P.Last_Name = @Instructor_Last_Name


	--Check assistant
	IF NOT EXISTS
	(
		SELECT
			P.Given_Name,
			P.Last_Name
		FROM Person AS P
		WHERE P.Given_Name = @Assistant_Given_Name
			AND P.Last_Name = @Assistant_Last_Name
	)
	BEGIN
		
		INSERT INTO Person (Given_Name, Last_Name)
		VALUES (@Assistant_Given_Name, @Assistant_Last_Name)
		INSERT INTO Assistant(Person_ID)
		SELECT P.Person_ID 
		FROM Person AS P 
		WHERE P.Given_Name = @Assistant_Given_Name 
			AND P.Last_Name = @Assistant_Last_Name	
	END
	--Get Assistant ID
	SELECT @Assistant_ID = A.Assistant_ID
	FROM Assistant AS A 
		INNER JOIN Person AS P
			ON A.Person_ID = P.Person_ID
	WHERE P.Given_Name = @Assistant_Given_Name
		AND P.Last_Name = @Assistant_Last_Name


	--Set Course Load
	INSERT INTO Course_Load (Course_ID, Section_ID, Instructor_ID, Assistant_ID, Term, [Year], Username, [Password])
	VALUES
	(@Course_ID, @Section_ID, @Instructor_ID, @Assistant_ID, @Term, @Year, @Username, @Password)

END

GO
/****** Object:  StoredProcedure [dbo].[spAddEquipment]    Script Date: 3/6/2017 3:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spAddEquipment]
(
	@Equipment_Name				VARCHAR(50),
	@Equipmentt_Quantity		INT
)
AS
BEGIN
	IF NOT EXISTS
	(
		SELECT 
		E.Equipment_Name
		FROM Equipment AS E
		WHERE E.Equipment_Name = @Equipment_Name
	)
	BEGIN
		--Set Experiment
		INSERT INTO Equipment(Equipment_Name, Equipment_Quantity)
		VALUES (@Equipment_Name, @Equipmentt_Quantity)
	END
	IF EXISTS
	(
		SELECT 
		E.Equipment_Name
		FROM Equipment AS E
		WHERE E.Equipment_Name = @Equipment_Name
	)
	BEGIN
		SELECT 'The Equipment already exists.' AS 'Return_Value'
	END
END

GO
/****** Object:  StoredProcedure [dbo].[spAddExperiment]    Script Date: 3/6/2017 3:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spAddExperiment] 
(
	@Experiment_Code				VARCHAR(10),
	@Experiment_Title				VARCHAR(50)
)
AS
BEGIN

	IF NOT EXISTS
	(
		SELECT 
		E.Experiment_Code
		FROM Experiment AS E
		WHERE E.Experiment_Code = @Experiment_Code
	)
	BEGIN
		--Set Experiment
		INSERT INTO Experiment (Experiment_Code, Experiment_Title)
		VALUES (@Experiment_Code, @Experiment_Title)
	END
	IF EXISTS
	(
		SELECT 
		E.Experiment_Code
		FROM Experiment AS E
		WHERE E.Experiment_Code = @Experiment_Code
	)
	BEGIN
		SELECT 'The Experiment Already Exists.' AS 'Return_Value'
	END

END

GO
/****** Object:  StoredProcedure [dbo].[spDeleteCourseSection]    Script Date: 3/6/2017 3:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spDeleteCourseSection]
(
	@Course_Code					VARCHAR(20),
	@Section_Code					VARCHAR(10),
	@Term							INT,
	@Year							INT,
	@Instructor_Given_Name			VARCHAR(100),
	@Instructor_Last_Name			VARCHAR(100),
	@Assistant_Given_Name			VARCHAR(100),
	@Assistant_Last_Name			VARCHAR(100),
	@Username						VARCHAR(50),
	@Password						VARCHAR(50)						
)
AS
BEGIN

	DECLARE @Course_ID				INT
	DECLARE @Section_ID				INT
	DECLARE @Instructor_ID			INT
	DECLARE @Assistant_ID			INT
	DECLARE @Person_ID				INT

	SELECT 
		@Course_ID = CL.Course_ID, 
		@Section_ID = CL.Section_ID,
		@Instructor_ID = CL.Instructor_ID, 
		@Assistant_ID = CL.Assistant_ID
	FROM Course_Load AS CL
		INNER JOIN Course AS C
			ON C.Course_ID = CL.Course_ID
		INNER JOIN Section AS S
			ON S.Section_ID = CL.Section_ID
		INNER JOIN Instructor  AS I
			ON I.Instructor_ID = CL.Instructor_ID
		INNER JOIN Assistant AS A
			ON A.Assistant_ID = CL.Assistant_ID
		INNER JOIN Person AS PIns
			ON PIns.Person_ID = I.Person_ID
		INNER JOIN Person AS PAss
			ON PAss.Person_ID = A.Person_ID
	WHERE 
		C.Course_Code = @Course_Code AND
		S.Section_Code = @Section_Code AND
		CL.Term = @Term AND
		CL.Year = @Year AND
		PIns.Given_Name = @Instructor_Given_Name AND
		PIns.Last_Name = @Instructor_Last_Name AND
		PAss.Given_Name = @Assistant_Given_Name AND
		PAss.Last_Name = @Assistant_Last_Name AND
		CL.Username = @Username AND
		CL.Password = @Password

	DELETE 
	FROM Course_Load
	WHERE
		Course_ID = @Course_ID AND
		Section_ID = @Section_ID AND
		Term = @Term AND
		Year = @Year AND
		Instructor_ID = @Instructor_ID AND
		Assistant_ID = @Assistant_ID AND
		Username = @Username AND
		Password = @Password

END

GO
/****** Object:  StoredProcedure [dbo].[spEditCourseSection]    Script Date: 3/6/2017 3:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spEditCourseSection]
(
	@Course_Load_ID					INT,
	@Course_Code					VARCHAR(20),
	@Section_Code					VARCHAR(10),
	@Term							INT,
	@Year							INT,
	@Instructor_Given_Name			VARCHAR(100),
	@Instructor_Last_Name			VARCHAR(100),
	@Assistant_Given_Name			VARCHAR(100),
	@Assistant_Last_Name			VARCHAR(100),
	@Username						VARCHAR(50),
	@Password						VARCHAR(50)
)
AS
BEGIN


	DECLARE @Course_ID				INT
	DECLARE @Section_ID				INT
	DECLARE @Instructor_ID			INT
	DECLARE @Assistant_ID			INT
	DECLARE @Person_ID				INT

	SELECT 
		@Course_ID = CL.Course_ID, 
		@Section_ID = CL.Section_ID,
		@Instructor_ID = CL.Instructor_ID, 
		@Assistant_ID = CL.Assistant_ID
	FROM Course_Load AS CL
		INNER JOIN Course AS C
			ON C.Course_ID = CL.Course_ID
		INNER JOIN Section AS S
			ON S.Section_ID = CL.Section_ID
		INNER JOIN Instructor  AS I
			ON I.Instructor_ID = CL.Instructor_ID
		INNER JOIN Assistant AS A
			ON A.Assistant_ID = CL.Assistant_ID
		INNER JOIN Person AS PIns
			ON PIns.Person_ID = I.Person_ID
		INNER JOIN Person AS PAss
			ON PAss.Person_ID = A.Person_ID
	WHERE 
		C.Course_Code = @Course_Code AND
		S.Section_Code = @Section_Code AND
		CL.Term = @Term AND
		CL.[Year] = @Year AND
		PIns.Given_Name = @Instructor_Given_Name AND
		PIns.Last_Name = @Instructor_Last_Name AND
		PAss.Given_Name = @Assistant_Given_Name AND
		PAss.Last_Name = @Assistant_Last_Name AND
		CL.Username = @Username AND
		CL.[Password] = @Password

	UPDATE Course_Load
	SET 
		Course_ID = @Course_ID,
		Section_ID = @Section_ID,
		Term = @Term,
		[Year] = @Year,
		Instructor_ID = @Instructor_ID,
		Assistant_ID = @Assistant_ID,
		Username = @Username,
		[Password] = @Password
	WHERE Course_Load_ID = @Course_Load_ID

END

GO
/****** Object:  StoredProcedure [dbo].[spGetCourseLoadID]    Script Date: 3/6/2017 3:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL

CREATE PROCEDURE [dbo].[spGetCourseLoadID]
(
	@Username						VARCHAR(50)
)
AS
BEGIN

	SELECT CL.Course_Load_ID 
	FROM Course_Load AS CL
	WHERE 
		CL.Username = @Username 

END

GO
/****** Object:  StoredProcedure [dbo].[spShowCourseLoad]    Script Date: 3/6/2017 3:36:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spShowCourseLoad]
(
	@Ignore					INT
)
AS
BEGIN
	
	SELECT 
		C.Course_Code AS 'Course', 
		S.Section_Code AS 'Section', 
		CAST(Year AS nvarchar) + ' - ' + CAST((Year+1) AS nvarchar) AS 'Year', 
		Term AS 'Term',  
		[dbo].[fnGetInstructrorName](I.Instructor_ID) AS 'Instructor', 
		[dbo].[fnGetAssistantName](A.Assistant_ID) AS 'Assistant',
		Username AS 'Username',
		Password AS 'Password'

	FROM Course_Load AS CL
		INNER JOIN Course AS C
			ON C.Course_ID = CL.Course_ID
		INNER JOIN Section AS S
			ON S.Section_ID = CL.Section_ID
		INNER JOIN Instructor  AS I
			ON I.Instructor_ID = CL.Instructor_ID
		INNER JOIN Assistant AS A
			ON A.Assistant_ID = CL.Assistant_ID
	ORDER BY CL.Course_Load_ID DESC, Term DESC, Year DESC

END

GO
