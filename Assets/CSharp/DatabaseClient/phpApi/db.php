<?php
header("Content-Type: application/json; charset=utf-8");

// XAMPP defaults
$host = "127.0.0.1";
$db   = "bulletheck";
$user = "root";
$pass = "";
// 

try {
  $pdo = new PDO("mysql:host=$host;dbname=$db;charset=utf8mb4", $user, $pass, [
    PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION
  ]);
} catch (Exception $e) {
  http_response_code(500);
  echo json_encode(["ok" => false, "error" => "DB connection failed"]);
  exit;
}
