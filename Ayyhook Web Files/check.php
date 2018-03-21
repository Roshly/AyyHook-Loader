<?php
$link = mysqli_connect("localhost", "mysql_username","mysql_password");
$database = mysqli_select_db($link, "mysql_database");

$user = $_GET['username'];
$password = $_GET['password'];
$hwid = $_GET['hwid'];
$tables = "mybb_users";

$sql = "SELECT * FROM ". $tables ." WHERE username = '". mysqli_real_escape_string($link,$user) ."'" ;
$result = $link->query($sql);
if ($result->num_rows > 0) {
    // Outputting the rows
    while($row = $result->fetch_assoc())
    {
        
        $password = $row['password'];
        $salt = $row['salt'];
        $plain_pass = $_GET['password'];
        $stored_pass = md5(md5($salt).md5($plain_pass));
        
        function Redirect($url, $permanent = false)
        {
            if (headers_sent() === false)
            {
                header('Location: ' . $url, true, ($permanent === true) ? 301 : 302);
            }
        exit();
        }
        
        if($stored_pass != $row['password'])
        {
            echo "p0<br>"; // Wrong pass, user exists
        }
        else
        {
            echo "p1<br>"; // Correct pass
        }
        
        echo "g" . $row['usergroup'] . "<br>";

        if (strlen($row['hwid']) > 1)
        {
            if ($hwid != $row['hwid'])
            {
                echo "h2"; // Wrong
            }
            else
            {
                echo "h1"; // Correct
            }
        }
        else
        {
            $sql = "UPDATE ". $tables ." SET hwid='$hwid' WHERE username='$user'";
            if(mysqli_query($link, $sql))
            {
                echo $row['hwid'];
                echo "h3"; // HWID Set
            }
            else
            {
                echo "h4"; // Else errors
            }
        }
    }
}  
?>