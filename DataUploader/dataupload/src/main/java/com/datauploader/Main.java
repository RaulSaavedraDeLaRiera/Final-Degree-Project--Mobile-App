package com.datauploader;

import java.io.*;
import java.net.HttpURLConnection;
import java.net.URL;
import java.nio.file.*;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.stream.Stream;

public class Main {
    public static void main(String[] args) {
        // Validar argumentos
        if (args.length < 2) {
            System.err.println("Uso: java -jar dataupload.jar <critterons|rooms|marks> <baseUrl>");
            return;
        }

        // Determinar modo según argumento
        String mode = args[0].toLowerCase();
        String baseUrl = args[1].endsWith("/") ? args[1].substring(0, args[1].length() - 1) : args[1];
        String folderPath;
        String apiUrl;

        switch (mode) {
            case "critterons":
                folderPath = "src/main/resources/Critterons";
                apiUrl = baseUrl + "/api/v1/critteron";
                break;
            case "marks":
                folderPath = "src/main/resources/Marks";
                apiUrl = baseUrl + "/api/v1/mark";
                break;
            case "rooms":
                folderPath = "src/main/resources/Rooms";
                apiUrl = baseUrl + "/api/v1/room";
                break;
            default:
                System.err.println("Modo inválido. Usa 'critterons', 'rooms' o 'marks' ");
                return;
        }

        String password = "1234567890qrtweu12h32i3o2nr23kj432mbr23kjeg32kjerg32ody2d8cUSUDAUbefgwfu23kweqhf";
        String hashedPassword = hashPassword(password);
        String token = getAuthToken(hashedPassword, baseUrl);

        if (token != null) {
            System.out.println("Token obtenido: " + token);

            try (Stream<Path> paths = Files.walk(Paths.get(folderPath))) {
                paths.filter(Files::isRegularFile)
                        .filter(path -> path.toString().endsWith(".json"))
                        .forEach(path -> sendJsonToApi(path, token, apiUrl));
            } catch (IOException e) {
                System.err.println("Error leyendo la carpeta: " + e.getMessage());
            }
        } else {
            System.err.println("Error obteniendo el token.");
        }
    }

    public static String getAuthToken(String hashedPassword, String baseUrl) {
        String apiUrl = baseUrl + "/api/v1/token?password=" + hashedPassword;
        try {
            URL url = new URL(apiUrl);
            HttpURLConnection conn = (HttpURLConnection) url.openConnection();
            conn.setRequestMethod("GET");
            conn.setRequestProperty("Content-Type", "application/json");

            int responseCode = conn.getResponseCode();
            if (responseCode == HttpURLConnection.HTTP_OK) {
                try (BufferedReader br = new BufferedReader(new InputStreamReader(conn.getInputStream(), "utf-8"))) {
                    StringBuilder response = new StringBuilder();
                    String responseLine;
                    while ((responseLine = br.readLine()) != null) {
                        response.append(responseLine.trim());
                    }
                    return response.toString();
                }
            } else {
                System.err.println("Error en la respuesta: " + responseCode);
            }
        } catch (IOException e) {
            System.err.println("Error en la solicitud: " + e.getMessage());
        }
        return null;
    }

    private static String hashPassword(String password) {
        try {
            MessageDigest digest = MessageDigest.getInstance("SHA-256");
            byte[] hash = digest.digest(password.getBytes());
            StringBuilder hexString = new StringBuilder();
            for (byte b : hash) {
                String hex = Integer.toHexString(0xff & b);
                if (hex.length() == 1)
                    hexString.append('0');
                hexString.append(hex);
            }
            return hexString.toString();
        } catch (NoSuchAlgorithmException e) {
            throw new RuntimeException("Error generando hash SHA-256", e);
        }
    }

    private static void sendJsonToApi(Path path, String token, String apiUrl) {
        try {
            String content = Files.readString(path);

            URL url = new URL(apiUrl);
            HttpURLConnection conn = (HttpURLConnection) url.openConnection();
            conn.setRequestMethod("POST");
            conn.setRequestProperty("Authorization", "Bearer " + token);
            conn.setRequestProperty("Content-Type", "application/json");
            conn.setDoOutput(true);

            try (OutputStream os = conn.getOutputStream()) {
                byte[] input = content.getBytes("utf-8");
                os.write(input, 0, input.length);
            }

            int responseCode = conn.getResponseCode();
            if (responseCode == HttpURLConnection.HTTP_OK || responseCode == HttpURLConnection.HTTP_CREATED) {
                System.out.println("Archivo enviado correctamente: " + path.getFileName());
            } else {
                System.err.println("Error enviando archivo " + path.getFileName() + ": " + responseCode);
            }
        } catch (IOException e) {
            System.err.println("Error procesando el archivo " + path.getFileName() + ": " + e.getMessage());
        }
    }
}
