var mysql = require("mysql");
var express = require('express');
var bodyParser = require('body-parser');
var nodemailer = require('nodemailer');
var app = express();
//Database connection
var con = mysql.createConnection({
  host: "us-cdbr-iron-east-05.cleardb.net",
  user: "bd3c486c768f8e",
  password: "53aaa9b2",
  database: "heroku_1b5e34ec020a60e"
});

//mailer creds


var send_mail = function(book_id, book_name, incorrect_location){
  var transporter = nodemailer.createTransport({
    service: 'gmail',
    auth: {
      user: '', //Add sender username
      pass: '' //Add sender password
    }
  });

  var mailOptions = {
  from: '', //Add sender username
  to: '',  //Add recipient username
  subject: `Notification for ${book_name}`,
  text: `Book with book id: ${book_id} and title: ${book_name} is in incorrect location at ${incorrect_location}`
  };

  transporter.sendMail(mailOptions, function(error, info){
  if (error) {
    console.log(error);
  } else {
    console.log('Email sent: ' + info.response);
  }
});
}

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
  var result = req.body;
  var book_id = result.book_id;
  console.log(book_id);
  console.log(result);
  var location = "fiction";
  console.log(`SELECT * FROM book_location WHERE book_id = ${book_id}`);
  con.query(`SELECT * FROM book_location WHERE book_id = ${book_id}`, function(err, result1, fields){
     if(err){
       console.log(err);
     } else {
       console.log(result1);
       if(result1[0].actual_location !== location){
         console.log("INCORRECT LO");
send_mail(result1[0].book_id, result1[0].book_name, result1[0].current_location);
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
