db = db.getSiblingDB('AuthDatabase'); // Nome do banco

db.users.insertMany([
    { email: "rafael@email.com", passwordHash: '1234' },
]);

var user = db.users.findOne({ email: "rafael@email.com" });
var userId = user._id;

db.companies.insertOne({
    name: "Rafael's Company",
    userId: userId // Referencing the user ID
});