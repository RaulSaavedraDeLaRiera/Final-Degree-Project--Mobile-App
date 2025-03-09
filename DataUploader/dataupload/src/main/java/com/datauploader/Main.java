package com.datauploader;

import java.io.*;
import java.net.HttpURLConnection;
import java.net.URL;
import java.nio.file.*;
import java.util.stream.Stream;

public class Main {
    public static void main(String[] args) {
        // Validar argumento
        if (args.length < 1) {
            System.err.println("Uso: java -jar dataupload.jar <critterons|rooms>");
            return;
        }

        // Determinar modo según argumento
        String mode = args[0].toLowerCase();
        String folderPath;
        String apiUrl;

        switch (mode) {
            case "critterons":
                folderPath = "src/main/resources/Critterons";
                apiUrl = "http://192.168.1.132:8080/api/v1/critteron";
                break;
            case "rooms":
                folderPath = "src/main/resources/Rooms";
                apiUrl = "http://192.168.1.132:8080/api/v1/room";
                break;
            default:
                System.err.println("Modo inválido. Usa 'critterons' o 'rooms'.");
                return;
        }

        String password = "1234567890qrtweu12h32i3o2nr23kj432mbr23kjeg32kjerg32ody2d8cUSUDAUbefgwfu23kweqhf";
        String token = getAuthToken(password);

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

    public static String getAuthToken(String password) {
        String apiUrl = "http://192.168.1.132:8080/api/v1/token?password=" + password;
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
