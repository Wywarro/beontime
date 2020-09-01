PROJECT_NAME ?= BEonTime

.PHONY: migrations db hello

migrations:
	cd ./BEonTime.Data && dotnet ef --startup-project ../BEonTime.Web/ migrations add $(mname) --context UserDbContext && dotnet ef --startup-project ../BEonTime.Web/ migrations add $(mname) --context ApplicationDbContext && cd ..

db:
	cd ./BEonTime.Data && dotnet ef --startup-project ../BEonTime.Web/ database update --context UserDbContext && dotnet ef --startup-project ../BEonTime.Web/ database update --context ApplicationDbContext && cd ..

hello:
	echo 'hello!'