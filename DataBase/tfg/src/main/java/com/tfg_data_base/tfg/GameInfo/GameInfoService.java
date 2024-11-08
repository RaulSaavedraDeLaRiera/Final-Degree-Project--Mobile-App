package com.tfg_data_base.tfg.GameInfo;
import org.springframework.stereotype.Service;

import lombok.RequiredArgsConstructor;

@Service
@RequiredArgsConstructor
public class GameInfoService {
    private final GameInfoRepository gameInfoRepository;

    private static final String GAME_INFO_ID = "unique_game_info_id";

    public GameInfo getGameInfo() {
        return gameInfoRepository.findById(GAME_INFO_ID)
            .orElse(new GameInfo());
    }

    public void setGameInfo(GameInfo gameInfo) {
        gameInfo.setId(GAME_INFO_ID);
        gameInfoRepository.save(gameInfo);
    }


    public void save(GameInfo gameInfo) {
        gameInfo.setId(GAME_INFO_ID);
        gameInfoRepository.save(gameInfo);
    }

    public void addCritteron(String critteronID) {
        GameInfo gameInfo = getGameInfo();
        boolean exists = gameInfo.getCritterons().stream()
            .anyMatch(critteron -> critteron.getCritteronID().equals(critteronID));
    
        if (!exists) {
            GameInfo.Critteron newCritteron = new GameInfo.Critteron();
            newCritteron.setCritteronID(critteronID);
            gameInfo.getCritterons().add(newCritteron);
            save(gameInfo);
        } else {
            System.out.println("El critteron con ID " + critteronID + " ya existe.");
        }
    }

    public void addUser(String userID) {
        GameInfo gameInfo = getGameInfo();
        boolean exists = gameInfo.getUsers().stream()
            .anyMatch(user -> user.getUserID().equals(userID));
        
        if (!exists) {
            GameInfo.User newUser = new GameInfo.User();
            newUser.setUserID(userID);
            gameInfo.getUsers().add(newUser);
            save(gameInfo);
        }else {
            System.out.println("El user con ID " + userID + " ya existe.");
        }
    }

    public void addForniture(String fornitureID) {
        GameInfo gameInfo = getGameInfo();
        boolean exists = gameInfo.getForniture().stream()
            .anyMatch(forniture -> forniture.getFornitureID().equals(fornitureID));
        
        if (!exists) {
            GameInfo.Forniture newForniture = new GameInfo.Forniture();
            newForniture.setFornitureID(fornitureID);
            gameInfo.getForniture().add(newForniture);
            save(gameInfo);
        }else {
            System.out.println("El forniture con ID " + fornitureID + " ya existe.");
        }
    }

    public void removeCritteron(String critteronID) {
        GameInfo gameInfo = getGameInfo();
        gameInfo.getCritterons().removeIf(critteron -> critteron.getCritteronID().equals(critteronID));
        save(gameInfo);
    }

    public void removeUser(String userID) {
        GameInfo gameInfo = getGameInfo();
        gameInfo.getUsers().removeIf(user -> user.getUserID().equals(userID));
        save(gameInfo);
    }

    public void removeForniture(String fornitureID) {
        GameInfo gameInfo = getGameInfo();
        gameInfo.getForniture().removeIf(forniture -> forniture.getFornitureID().equals(fornitureID));
        save(gameInfo);
    }

}
