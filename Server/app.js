var mysql = require("mysql");
var express = require('express');
var bodyParser = require('body-parser');

var app = express();
//Database connection

var con = mysql.createConnection({
  host: "localhost",
  user: "root",
  password: "root",
  database: "library_data"
});

app.use(bodyParser.json());
app.post('/', (req, res) => {
	console.log('Index Page');
	//res.send('world');

  	con.query("SELECT * FROM book_location", function (err, result, fields) {
    if (err) throw err;
    res.send(result);
  	});

});

app.post('/check', (req, res) => {
	var test = req.body;
	//res.send(JSON.parse(test));
	console.log("Check Page");
	console.log(req.body);
	res.send(test);
});
//app.use('/books', books);

app.listen(3000);
//module.exports = app;
