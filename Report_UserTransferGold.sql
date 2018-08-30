-- result
--	TotalGoldSend
--	TotalGoldRecieve
--	DateSend
create proc usp_ReportTransferGold_GetData
	@DateFrom date,
	@DateTo date,
	@UserSearch nvarchar(150),
	@TypeSearch int = 0
as
begin
	if @TypeSearch = 0
	begin
		select ISNULL(SUM(t.TotalGoldSend),0) as TotalGoldSend, ISNULL(SUM(t.TotalGoldSend - t.TotalGoldFee),0) as TotalGoldRecieve, t.DateSend
		from Report_UserTransferGold as t
		where t.DateSend between @DateFrom and @DateTo
		group by t.DateSend
		order by t.DateSend
	end
	else
	begin
		declare @GoldSend Table(
			TotalGoldSend money,
			DateSend date)

		declare @GoldRecieve Table(
			TotalGoldRecieve money,
			DateSend date)
		if @TypeSearch = 1
			declare @UserID int = Convert(int,@UserSearch)
		insert into @GoldSend
			select SUM(t.TotalGoldSend) as TotalGoldSend, DateSend
			from Report_UserTransferGold as t
			where t.UserSend = (case @TypeSearch	
				when 1 then @UserID
				else t.UserSend
				end) and t.UserSendName = (case @TypeSearch
				when 1 then t.UserSendName
				else @UserSearch
				end) and t.DateSend between @DateFrom and @DateTo
			group by DateSend
			order by DateSend
		insert into @GoldRecieve
			select SUM(t.TotalGoldSend - t.TotalGoldFee) as TotalGoldSend, DateSend
			from Report_UserTransferGold as t
			where t.UserRecieve = (case @TypeSearch	
				when 1 then @UserID
				else t.UserRecieve
				end) and t.UserRecieveName = (case @TypeSearch
				when 1 then t.UserRecieveName
				else @UserSearch
				end) and t.DateSend between @DateFrom and @DateTo
			group by DateSend
			order by DateSend

		select ISNULL(s.TotalGoldSend,0) as TotalGoldSend, ISNULL(r.TotalGoldRecieve,0) as TotalGoldRecieve, ISNULL(s.DateSend,r.DateSend) as DateSend
		from @GoldSend as s full outer join @GoldRecieve as r on s.DateSend = r.DateSend
	end
end
--declare @DateFrom date = '2018-08-01', @DateTo date = '2018-08-03'
--declare @UserSearch nvarchar(150) = '11' , @TypeSearch int = 0 -- typesearch = 0 khong co user ; 1 usersearch la userid ; 2 usersearch la username