package com.tfg_data_base.tfg.GameInfo;

import org.springframework.web.bind.annotation.RequestMapping;
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
