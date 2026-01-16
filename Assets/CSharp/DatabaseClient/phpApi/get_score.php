<?php
require_once "db.php";

$raw = file_get_contents("php://input");
$data = json_decode($raw, true);

if (!is_array($data) || !isset($data["id"])) {
  http_response_code(400);
  echo json_encode(["ok" => false, "error" => "Missing id"]);
  exit;
}

$id = $data["id"] ;

$stmt = $pdo->prepare("SELECT `id`, `player1`, `player2`, `score`, `created_at` FROM scores WHERE `id` LIKE ?");
$stmt->execute([$id]);
$rows = $stmt->fetchAll();

echo json_encode(["ok" => true, "score" => $rows[0]]);
