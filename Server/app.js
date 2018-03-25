var mysql = require("mysql");
var express = require('express');
var bodyParser = require('body-parser');

var app = express();
//Database connection

var con = mysql.createConnection({
  host: "us-cdbr-iron-east-05.cleardb.net",
  user: "bd3c486c768f8e",
  password: "53aaa9b2",
  database: "heroku_1b5e34ec020a60e"
});

app.use(bodyParser.json());

app.get('/', (req, res) => {
	console.log('Index Page');
  res.send("App is running!!!!");
});

app.get('/all', (req, res) => {
	console.log('Index Page');
	//res.send('world');

  	con.query("SELECT * FROM book_location", function (err, result, fields) {
    if (err) throw err;
    res.send(result);
  	});

});

app.post('/all', (req, res) => {
	console.log('/all Page');
	//res.send('world');

  	con.query("SELECT * FROM book_location", function (err, result, fields) {
    if (err) throw err;
    res.send(result);
  	});

});

app.post('/check', (req, res) => {
	var test = req.body;
	//res.send(JSON.parse(test));
	//console.log("Check Page");
	//console.log(req.body);
  var result = req.body;
  //res.send(result);
  var book_id = result.book_id;
  console.log(book_id);
  //res.send(book_id);
  console.log(result);
  var location = "fiction";
  console.log(`SELECT * FROM book_location WHERE book_id = ${book_id}`);
  con.query(`SELECT * FROM book_location WHERE book_id = ${book_id}`, function(err, result1, fields){

     if(err){
       console.log(err);
     } else {
       console.log(result1);
       if(result1[0].actual_location !== location){
         console.log(book_id);
         con.query(`UPDATE book_location set is_location_correct=false WHERE book_id = ${book_id}`);
      } else {
        con.query(`UPDATE book_location set is_location_correct=true WHERE book_id = ${book_id}`);
      }
     }
   });

	res.send(test);
});
//app.use('/books', books);

var port = process.env.PORT || 3000;
app.listen(port, "0.0.0.0", function() {
console.log("Listening on Port 3000");
});
//module.exports = app;
