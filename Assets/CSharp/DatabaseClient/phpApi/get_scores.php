<?php
require_once "db.php";

$stmt = $pdo->query("SELECT id, player1, player2, score, created_at FROM scores ORDER BY id DESC LIMIT 20");
$rows = $stmt->fetchAll(PDO::FETCH_ASSOC);

echo json_encode(["ok" => true, "scores" => $rows]);
