UPDATE
	tblPostImage
SET RawName = REPLACE(ImageUrl, 'Content/Files/', '') FROM tblPostImage 

GO

SELECT * 
FROM tblPostImage