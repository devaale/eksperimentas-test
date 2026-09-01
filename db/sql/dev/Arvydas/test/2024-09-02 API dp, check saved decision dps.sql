select dv.*
from tblDatapoint dp
inner join tblDatapointValue dv on dv.DatapointId = dp.Id
where Alias = 'decision'