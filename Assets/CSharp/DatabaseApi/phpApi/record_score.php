<?php
require_once "db.php";

$raw = file_get_contents("php://input");
$data = json_decode($raw, true);

if (!is_array($data) || !isset($data["player1"])) {
  http_response_code(400);
  echo json_encode(["ok" => false, "error" => "Missing player1"]);
  exit;
}

$player1 = $data["player1"] ;


if (!is_array($data) || !isset($data["player2"])) {
  http_response_code(400);
  echo json_encode(["ok" => false, "error" => "Missing player2"]);
  exit;
}

$player2 = $data["player2"] ;


if (!is_array($data) || !isset($data["player1score"])) {
  http_response_code(400);
  echo json_encode(["ok" => false, "error" => "Missing player1score"]);
  exit;
}

$player1score = $data["player1score"] ;


if (!is_array($data) || !isset($data["player2score"])) {
  http_response_code(400);
  echo json_encode(["ok" => false, "error" => "Missing player2score"]);
  exit;
}

$player2score = $data["player2score"] ;


$stmt = $pdo->prepare("INSERT INTO scores (player1,player2,player1score,player2score) VALUES (?,?,?,?)");
$stmt->execute([$player1,$player2,$player1score,$player2score]);

echo json_encode(["ok" => true, "id" => $pdo->lastInsertId()]);
