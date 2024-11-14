package com.tfg_data_base.tfg.GameInfo;

import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import lombok.RequiredArgsConstructor;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;

@RestController
@RequestMapping("/api/v1")
@RequiredArgsConstructor
public class GameInfoController {
    private final GameInfoService gameInfoService;

    @GetMapping("/gameinfo")
    public GameInfo getGameInfo() {
        return gameInfoService.getGameInfo();
    }

    @PostMapping("/gameinfo")
    public void setGameInfo(@RequestBody GameInfo gameInfo) {
        gameInfoService.setGameInfo(gameInfo);
    }

    @PostMapping("/gameinfo/initialize")
    public String initializeGameInfo() {
        gameInfoService.initializeEmptyGameInfo();
        return "GameInfo initialized with empty lists and 7 days of rewards.";
    }

    @PostMapping("/gameinfo/weekRewards")
    public void updateWeekRewards(@RequestBody GameInfo.WeekRewards weekRewards) {
        gameInfoService.updateWeekRewards(weekRewards);
    }

    @PostMapping("/gameinfo/weekRewards/{day}")
    public void updateDayReward(@PathVariable String day, @RequestParam Integer reward1, @RequestParam Integer reward2) {
        gameInfoService.updateDayReward(day, reward1, reward2);
    }

    public void addCritteron(@PathVariable String critteronId) {
        gameInfoService.addCritteron(critteronId);
    }

    public void removeCritteron(@PathVariable String critteronId) {
        gameInfoService.removeCritteron(critteronId);
    }

    public void addUser(@PathVariable String userId) {
        gameInfoService.addUser(userId);
    }

    public void removeUser(@PathVariable String userId) {
        gameInfoService.removeUser(userId);
    }
   
    public void addForniture(@PathVariable String fornitureId) {
        gameInfoService.addForniture(fornitureId);
    }
  
    public void removeForniture(@PathVariable String fornitureId) {
        gameInfoService.removeForniture(fornitureId);
    }
}
