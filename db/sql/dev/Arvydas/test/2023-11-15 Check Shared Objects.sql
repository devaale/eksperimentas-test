SELECT *
  FROM tblObjectPermission OP
  inner join tblObject O on O.Id = OP.ObjectId
  inner join AspNetUsers u on u.Id = Op.FriendUserId