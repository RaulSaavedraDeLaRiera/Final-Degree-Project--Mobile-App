package com.tfg_data_base.tfg.GameInfo;

import java.util.HashMap;
import java.util.Map;

import org.springframework.stereotype.Service;

import lombok.RequiredArgsConstructor;

@Service
@RequiredArgsConstructor
public class GameInfoService {
    private final GameInfoRepository gameInfoRepository;

    public GameInfo getGameInfo() {
        return gameInfoRepository.findById("GAME_INFO_ID")
                .orElseGet(() -> {
                    GameInfo newGameInfo = new GameInfo();
                    newGameInfo.setId("GAME_INFO_ID");
                    gameInfoRepository.save(newGameInfo);
                    return newGameInfo;
                });
    }

    public void updateWeekRewards(GameInfo.WeekRewards newWeekRewards) {
        GameInfo gameInfo = getGameInfo();
        gameInfo.setWeekRewards(newWeekRewards);
        gameInfoRepository.save(gameInfo);
    }

    public void updateDayReward(String day, Integer reward1, Integer reward2) {
        GameInfo gameInfo = getGameInfo();
        if (gameInfo.getWeekRewards() == null) {
            gameInfo.setWeekRewards(new GameInfo.WeekRewards());
        }

        Map<String, GameInfo.WeekRewards.DayReward> days = gameInfo.getWeekRewards().getDays();
        if (days == null) {
            days = new HashMap<>();
            gameInfo.getWeekRewards().setDays(days);
        }

        days.put(day, new GameInfo.WeekRewards.DayReward(reward1, reward2));
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
            gameInfoRepository.save(gameInfo);
        } else {
            System.out.println("Critteron with ID " + critteronID + " already exists in the data base");
        }
    }

    public void addRoom(String roomID) {
        GameInfo gameInfo = getGameInfo();
        boolean exists = gameInfo.getRooms().stream()
                .anyMatch(furniture -> furniture.getRoomID().equals(roomID));

        if (!exists) {
            GameInfo.Room newRoom = new GameInfo.Room();
            newRoom.setRoomID(roomID);
            gameInfo.getRooms().add(newRoom);
            gameInfoRepository.save(gameInfo);
        } else {
            System.out.println("Room with ID " + roomID + " already exists in the data base");
        }
    }

    public void addMark(String markID) {
        GameInfo gameInfo = getGameInfo();

        boolean exists = gameInfo.getMarks().stream()
                .anyMatch(mark -> mark.getMarkID().equals(markID));

        if (!exists) {
            GameInfo.Mark newMark = new GameInfo.Mark();
            newMark.setMarkID(markID);
            gameInfo.getMarks().add(newMark);
            gameInfoRepository.save(gameInfo);
        } else {
            System.out.println("Mark with ID " + markID + " already exists in the data base");
        }
    }

    public void removeCritteron(String critteronID) {
        GameInfo gameInfo = getGameInfo();
        gameInfo.getCritterons().removeIf(critteron -> critteron.getCritteronID().equals(critteronID));
        gameInfoRepository.save(gameInfo);
    }

    public void removeMark(String markID) {
        GameInfo gameInfo = getGameInfo();
        gameInfo.getMarks().removeIf(mark -> mark.getMarkID().equals(markID));
        gameInfoRepository.save(gameInfo);
    }

    public void removeRoom(String roomId) {
        GameInfo gameInfo = getGameInfo();
        gameInfo.getRooms().removeIf(furniture -> furniture.getRoomID().equals(roomId));
        gameInfoRepository.save(gameInfo);
    }

    /**
     * Inicializa el json comprobando que no existiera antes uno ya
     */
    public void initializeEmptyGameInfo() {
        GameInfo gameInfo = gameInfoRepository.findById("GAME_INFO_ID")
                .orElse(null);

        if (gameInfo == null) {
            GameInfo emptyGameInfo = new GameInfo();
            emptyGameInfo.setId("GAME_INFO_ID");
            GameInfo.WeekRewards weekRewards = new GameInfo.WeekRewards();
            Map<String, GameInfo.WeekRewards.DayReward> days = new HashMap<>();
            for (int i = 1; i <= 7; i++) {
                days.put(String.valueOf(i), new GameInfo.WeekRewards.DayReward(0, 0));
            }
            weekRewards.setDays(days);
            emptyGameInfo.setWeekRewards(weekRewards);
            emptyGameInfo.setCureTime(500);  
            emptyGameInfo.setMarkTime(100);  
            emptyGameInfo.setExpGoal(100);
            emptyGameInfo.setReward(10);
            emptyGameInfo.setExpPerCombat(20);
            emptyGameInfo.setStepsToCombat(150);
            gameInfoRepository.save(emptyGameInfo);
        }
    }
}
