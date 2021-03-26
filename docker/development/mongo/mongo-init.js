db.createUser({
  user: "root",
  pwd: "rootpassword",
  roles: [
    {
      role: "readWrite",
      db: "beontime_dev"
    }
  ]
});
