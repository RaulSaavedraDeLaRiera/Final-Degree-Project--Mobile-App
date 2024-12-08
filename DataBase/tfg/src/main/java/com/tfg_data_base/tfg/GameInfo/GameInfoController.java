package com.tfg_data_base.tfg.GameInfo;

import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import lombok.RequiredArgsConstructor;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;

import java.util.Map;


@RestController
@RequestMapping("/api/v1")
@RequiredArgsConstructor
public class GameInfoController {
    private final GameInfoService gameInfoService;

    /**
     * Obtengo el gameInfo
     * @return
     */
    @GetMapping("/gameinfo")
    public GameInfo getGameInfo() {
        return gameInfoService.getGameInfo();
    }

    /**
     * Crea en la base de datos un gameinfo inicial
     * @return
     */
    @PostMapping("/gameinfo/initialize")
    public String initializeGameInfo() {
        gameInfoService.initializeEmptyGameInfo();
        return "GameInfo initialized";
    }

    /**
     * Modificar las recompensas semanales en conjunto
     * @param weekRewards
     */
    @PostMapping("/gameinfo/weekRewards")
    public void updateWeekRewards(@RequestBody GameInfo.WeekRewards weekRewards) {
        gameInfoService.updateWeekRewards(weekRewards);
    }

    /**
     * Modificar las recompensas semanales de un dia en especifico
     * @param day
     * @param reward1
     * @param reward2
     */
    @PostMapping("/gameinfo/weekRewards/{day}")
    public void updateDayReward(@PathVariable String day, @RequestBody Map<String, Integer> rewards) {
        Integer reward1 = rewards.get("reward1");
        Integer reward2 = rewards.get("reward2");
        gameInfoService.updateDayReward(day, reward1, reward2);
    }


    public void addCritteron(@PathVariable String critteronId) {
        gameInfoService.addCritteron(critteronId);
    }

    public void removeCritteron(@PathVariable String critteronId) {
        gameInfoService.removeCritteron(critteronId);
    }

    public void addRoom(@PathVariable String roomId) {
        gameInfoService.addRoom(roomId);
    }
  
    public void removeRoom(@PathVariable String roomId) {
        gameInfoService.removeRoom(roomId);
    }
}
