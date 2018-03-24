var app = require('../app');

router.get('/', function(req, res, next) {
	res.locals.connection.query('SELECT * from book_location', function (error, results, fields) {
		if (error) throw error;
    console.log(res);
		res.send(JSON.stringify({"status": 200, "error": null, "response": results}));
	});
});
