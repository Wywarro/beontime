PROJECT_NAME ?= BIMonTime

.PHONY: migrations db hello

migrations:
	cd ./BIMonTime.Data && dotnet ef --startup-project ../BIMonTime.Web/ migrations add $(mname) --context UserDbContext && dotnet ef --startup-project ../BIMonTime.Web/ migrations add $(mname) --context ApplicationDbContext && cd ..

db:
	cd ./BIMonTime.Data && dotnet ef --startup-project ../BIMonTime.Web/ database update --context UserDbContext && dotnet ef --startup-project ../BIMonTime.Web/ database update --context ApplicationDbContext && cd ..

hello:
	echo 'hello!'