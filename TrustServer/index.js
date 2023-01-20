const express = require('express');
const app = express();
//create a sqlite3 database
const sqlite3 = require('sqlite3').verbose();
const bodyParser = require('body-parser');
const fs = require('fs');


//try to connect to db.db. if it does not exist, create it
let db = new sqlite3.Database('db.sqlite', (err) => {
  if (err) {
    console.log(err.message);
  }
  console.log('Connected to the db database.');
});


//create a table
db.serialize(function() {
  //create a table for players which contains their playerid, x and y coordinates, health, and AP
  db.run("CREATE TABLE IF NOT EXISTS players (playerid INTEGER PRIMARY KEY, x INTEGER, y INTEGER, health INTEGER, AP INTEGER, colorR REAL, colorG REAL, colorB REAL)");
});


//get settings from settings.json file and store them in variables
const settings = JSON.parse(fs.readFileSync('settings.json', 'utf8'));  
const mapSize = settings.mapSize;
const range = settings.range;
const allowDebug = settings.allowDebug;
const rps = settings.rps;
const port = settings.port;





app.get('/settings', (req, res) => {
  //res.send('{"mapSize":[10,10],"range":5}');
  res.send('{"mapSize":['+ mapSize +','+ mapSize +'],"range":'+ range +', "allowDebug":'+ allowDebug +', "rps":'+ rps +'}');
  res.end();
  console.log('Settings requested');
});

app.get('/map', (req, res) => {
  //return all players in the database
  db.all("SELECT * FROM players", function(err, rows) {
    //console.log(rows);
    if (err) console.log(err);
    res.send(rows);
    res.end();
  });
});

app.post('/debug_setAP', bodyParser.urlencoded({extended: false}), (req, res) => {
  //add AP to a player
  if(allowDebug) {
    db.serialize(function() {
      db.run("UPDATE players SET AP = " + req.body.AP + " WHERE playerid = " + req.body.playerid);
      res.send('[true]');
      res.end();
    });
    console.log('DEBUG: Player ' + req.body.playerid + ' AP set to ' + req.body.AP);
  }
});

app.post('/debug_setHealth', bodyParser.urlencoded({extended: false}), (req, res) => {
  //add health to a player
  if(allowDebug) {
    db.serialize(function() {
      db.run("UPDATE players SET health = " + req.body.health + " WHERE playerid = " + req.body.playerid);
      res.send('[true]');
      res.end();
    });
    console.log('DEBUG: Player ' + req.body.playerid + ' health set to ' + req.body.health);
  }
});


app.post('/join', bodyParser.urlencoded({extended: false}), (req, res) => {
  //filter out players in database by playerid to find if the player is already in the database. if they are, return their data. if not, add them to the database
  db.all("SELECT * FROM players WHERE playerid = " + req.body.playerid, function(err, rows) {
    if (err) console.log(err);
    if (rows == "") {
      res.send('[false]');
      res.end();
    } else {
      res.send(rows);
      res.end();
    }
  });
  console.log('Player ' + req.body.playerid + ' reconnected');
});

app.get('/ping', (req, res) => {
  res.send('[true]');
  res.end();
});

app.get('/newjoin', (req, res) => {
  //add a player to the database

  //check how many players are in the database and store the number in a variable
  db.all("SELECT * FROM players", function(err, rows) {
    if (err) console.log(err);
    var playerCount = rows.length;
    console.log('playerCount: ' + playerCount);
    if(playerCount < settings.maxPlayers) {
      db.serialize(function() {
        //insert a player into the database
        var playerid = Math.floor(Math.random() * 1000000000);
        var x = Math.floor(Math.random() * 10);
        var y = Math.floor(Math.random() * 10);
        var colorR = Math.random();
        var colorG = Math.random();
        var colorB = Math.random();
        var health = 3;
        var AP = 3;
        db.run("INSERT INTO players (playerid, x, y, health, AP, colorR, colorG, colorB) VALUES ("+ playerid +" , "+ x +" , "+ y +" , "+ health +" , "+ AP +", "+ colorR +", "+ colorG +", "+ colorB +")");
        res.send('{"playerid":'+ playerid +',"x":'+ x +',"y":'+ y +',"health":'+ health +',"AP":'+ AP +',"color": ['+ colorR +', '+ colorG +', '+ colorB +']}');
        console.log('Player ' + playerid + ' joined');
      });
    } else {
      res.status(418).send('{"error":"Too many players"}');
      res.end();
    }
  });
});

app.post('/move', bodyParser.urlencoded({extended: false}), (req, res) => {
  //move a player
  db.serialize(function() {
    db.run("UPDATE players SET x = " + req.body.x + ", y = " + req.body.y + ", AP = " + req.body.AP + " WHERE playerid = " + req.body.playerid);
    res.send('[true]');
    res.end();
  });
  console.log('Player ' + req.body.playerid + ' moved to (' + req.body.x + ', ' + req.body.y + ')');
});

app.post('/attack', bodyParser.urlencoded({extended: false}), (req, res) => {
  //attack a player
  db.serialize(function() {

    db.run("UPDATE players SET health = " + req.body.targetHealth + " WHERE playerid = " + req.body.targetid); 
    console.log('Player ' + req.body.targetid + ' health: ' + req.body.targetHealth);
    console.log('eval: ' + (req.body.targetHealth <= 0))
    if (req.body.targetHealth <= 0) {
      //set health back to 3, color back to white, and playerid back to -1
      db.run("UPDATE players SET health = 3, colorR = 1, colorG = 1, colorB = 1, playerid = -1 WHERE playerid = " + req.body.targetid);
      console.log('Player ' + req.body.targetid + ' died');
    } else {
    }
    console.log('targetHealth: ' + req.body.targetHealth);
    
  });

  db.serialize(function() {
    db.run("UPDATE players SET AP = " + req.body.AP + " WHERE playerid = " + req.body.playerid);
  });
  console.log('Player ' + req.body.playerid + ' attacked player ' + req.body.targetid);
  if (req.body.health == 0) {
    console.log('Player ' + req.body.playerid + ' died');
  }
  res.send('[true]');
  res.end();
});

app.post('/giveap', bodyParser.urlencoded({extended: false}), (req, res) => {
  playerAP = req.body.AP;
  playerID = req.body.playerid;
  targetID = req.body.targetid;


  //look up the target's AP
  db.all("SELECT * FROM players WHERE playerid = " + targetID, function(err, rows) {
    if (err) console.log(err);
    //define the target's AP

    targetAP = rows[0].AP;
    targetAP++;
    console.log(targetAP);
    //update the target's AP
    db.serialize(function() {
      db.run("UPDATE players SET AP = " + targetAP + " WHERE playerid = " + targetID);
    });
  });

  //update the player's AP
  db.serialize(function() {
    db.run("UPDATE players SET AP = " + playerAP + " WHERE playerid = " + playerID);
  });
  res.send('[true]');
  res.end();
});
let round = 0;
//create a loop that runs every x seconds to add AP to all players
function run() {
  setInterval(function() { 
    //add AP to all players
    round++;
    console.log('round: ' + round);

    
    
    db.serialize(function() {
      //if any players have a playerid of -1, delete them
      db.run("DELETE FROM players WHERE playerid = -1");

      //add AP to all players
      db.run("UPDATE players SET AP = AP + 1");
    });
 }, settings.secondsBetweenRounds * 1000);
}

console.log(`
▀█▀ █▀█ █░█ █▀ ▀█▀   █▀ █▀▀ █▀█ █░█ █▀▀ █▀█
░█░ █▀▄ █▄█ ▄█ ░█░   ▄█ ██▄ █▀▄ ▀▄▀ ██▄ █▀▄
-------------------------------------------`)


app.listen(settings.port, () => console.log('Server started on port ' + settings.port));
run();
