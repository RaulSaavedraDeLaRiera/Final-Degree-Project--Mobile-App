package com.tfg_data_base.tfg;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class ConectorController {

    @GetMapping("/connected")
    public String healthCheck() {
        return "Server fully conected";
    }
}