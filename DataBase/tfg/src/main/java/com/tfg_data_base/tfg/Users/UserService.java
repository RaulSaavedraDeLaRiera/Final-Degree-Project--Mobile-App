package com.tfg_data_base.tfg.Users;

import java.util.List;
import java.util.Optional;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.mongodb.core.MongoTemplate;
import org.springframework.data.mongodb.core.query.Criteria;
import org.springframework.stereotype.Service;
import org.springframework.data.mongodb.core.query.Query;
import org.springframework.data.mongodb.core.query.Update;

import com.tfg_data_base.tfg.GameInfo.GameInfoService;

import lombok.RequiredArgsConstructor;

@Service
@RequiredArgsConstructor
public class UserService {
    private final UserRepository userRepository;
    private final GameInfoService gameInfoService;

    @Autowired
    private MongoTemplate mongoTemplate;


    public void save(User user) {
        userRepository.save(user);
        gameInfoService.addUser(user.getId());
    }

    public List<User> findAll() {
        return userRepository.findAll();
    }

    public Optional<User> findById(String id) {
        return userRepository.findById(id);
    }

    public void deleteById(String id) {
        userRepository.deleteById(id);
        gameInfoService.removeUser(id);
    }

    public void updateUserField(String userId, String fieldName, Object newValue) {
        Query query = new Query(Criteria.where("id").is(userId));
        Update update = new Update().set(fieldName, newValue);
        mongoTemplate.updateFirst(query, update, User.class);
    }
}