package com.tfg_data_base.tfg.Users;

import java.util.List;
import java.util.Map;
import java.util.ArrayList;
import java.util.Optional;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.mongodb.core.MongoTemplate;
import org.springframework.data.mongodb.core.query.Criteria;
import org.springframework.stereotype.Service;
import org.springframework.data.mongodb.core.query.Query;
import org.springframework.data.mongodb.core.query.Update;
import org.springframework.security.core.context.SecurityContextHolder;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.tfg_data_base.tfg.Security.JwtUtils;
import com.tfg_data_base.tfg.UserInfo.UserInfoService;
import com.tfg_data_base.tfg.Users.User.CritteronUser;
import com.tfg_data_base.tfg.Users.User.RoomOwned;

import org.springframework.security.core.Authentication;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.core.userdetails.UserDetails;

import lombok.RequiredArgsConstructor;

@Service
@RequiredArgsConstructor
public class UserService {
    private final UserRepository userRepository;
    private final UserInfoService userInfoService;

    private JwtUtils jwtUtils = new JwtUtils();
    private String userPasswordCredentials = "Jm7N@q9!Xf2#ZlT6pV";

    @Autowired
    private MongoTemplate mongoTemplate;

    /**
     * Se llama cuando se crea un nuevo usuario
     * @param user
     */
    public void save(User user) {

        if (userRepository.findByMail(user.getMail()).isPresent()) {
            throw new IllegalArgumentException("Mail in use");
        }

        if (user.getUserData() == null) {
            user.setUserData(new User.UserData());
        }
    
        if (user.getSocialStats() == null) {
            user.setSocialStats(new ArrayList<>());
        }
    
        if (user.getPersonalStats() == null) {
            user.setPersonalStats(new User.PersonalStats());
        }
    
        if (user.getCritterons() == null) {
            user.setCritterons(new ArrayList<>());
        }
    
        if (user.getRoomOwned() == null) {
            user.setRoomOwned(new ArrayList<>());
        }

        if (user.getMail() == null) user.setMail("");
        if (user.getPassword() == null) user.setPassword("");
    
        User.UserData userData = user.getUserData();
        if (userData.getName() == null) userData.setName("");
        if (userData.getLevel() == null) userData.setLevel(0);
        if (userData.getExperience() == null) userData.setExperience(0);
        if (userData.getMoney() == null) userData.setMoney(0);
        if (userData.getCurrentCritteron() == null) userData.setCurrentCritteron("");
    
        User.PersonalStats personalStats = user.getPersonalStats();
        if (personalStats.getGlobalSteps() == null) personalStats.setGlobalSteps(0);
        if (personalStats.getDaysStreak() == null) personalStats.setDaysStreak(0);
        if (personalStats.getWeekSteps() == null) personalStats.setWeekSteps(0);
        if (personalStats.getCombatWins() == null) personalStats.setCombatWins(0);
        if (personalStats.getCritteronsOwned() == null) personalStats.setCritteronsOwned(0);
        if (personalStats.getPercentHotel() == null) personalStats.setPercentHotel(0);

        userRepository.save(user);
        userInfoService.addUser(user.getId());
    }

    public String login(String mail, String password)
    {
        Query query = new Query();
        query.addCriteria(Criteria.where("mail").is(mail));
        query.fields().include("password").include("mail"); 

        // Encuentra al usuario
        Map<String, Object> user = mongoTemplate.findOne(query, Map.class, "User");

        // Si no se encuentra el usuario, retorna false
        if (user != null && password.equals((String) user.get("password"))) {
            return  jwtUtils.generateJwtToken(mail);
        }

      return "";
    }

    public String getUserIdByCredentials(String mail, String password) {
        User user = userRepository.findByMailAndPassword(mail, password);

        if (user != null) {
            return user.getId();
        }

        return "";
    }
   
    public List<User> findAll() {
        return userRepository.findAll();
    }

    public Optional<User> findById(String id) {
        return userRepository.findById(id);
    }

    public void deleteById(String id) {
        if(!verifyUser(id))
            throw new SecurityException("Cant modify a different user"); 

        userRepository.deleteById(id);
        userInfoService.removeUser(id);
    }

    /**
     * Actualizar un parametro de un usuario en concreto
     * @param userId
     * @param fieldName
     * @param newValue
     */
    public void updateUserField(String userId, String fieldName, Object newValue) {
      
        if(!verifyUser(userId))
            throw new SecurityException("Cant modify a different user");


        Query query = new Query(Criteria.where("id").is(userId));

        // Modificar / añadir critteron
        if ("critterons".equals(fieldName)) {
            CritteronUser newCritteron = new ObjectMapper().convertValue(newValue, CritteronUser.class);
            Query critteronQuery = new Query(Criteria.where("id").is(userId)
                .and("critterons.critteronID").is(newCritteron.getCritteronID()));
            
            if (mongoTemplate.exists(critteronQuery, User.class)) {
                Update update = new Update()
                    .set("critterons.$.level", newCritteron.getLevel())
                    .set("critterons.$.currentLife", newCritteron.getCurrentLife())
                    .set("critterons.$.startInfo", newCritteron.getStartInfo());
                
                mongoTemplate.updateFirst(critteronQuery, update, User.class);
            } else {
                mongoTemplate.updateFirst(query, new Update().addToSet("critterons", newCritteron), User.class);
            }
        // Añadir mueble comprado
        } else if ("roomOwned".equals(fieldName)) {
            String newRoomID = (String) newValue; 
            RoomOwned newRoom = new RoomOwned(newRoomID);
            Update update = new Update().addToSet("roomOwned", newRoom);
            mongoTemplate.updateFirst(query, update, User.class);
        } else if ("socialStats".equals(fieldName)) {
            String newSocialStatID = (String) newValue; 
            Update update = new Update().addToSet("socialStats", new User.SocialStat(newSocialStatID));
            mongoTemplate.updateFirst(query, update, User.class);
        }
        else {
            Update update = new Update().set(fieldName, newValue);
            mongoTemplate.updateFirst(query, update, User.class);
        }
    }   
    
    public void removeFriend(String userId, String friendID) {

        if(!verifyUser(userId))
            throw new SecurityException("Cant modify a different user");

        Query query = new Query(Criteria.where("id").is(userId));
        Update update = new Update().pull("socialStats", new User.SocialStat(friendID));
        mongoTemplate.updateFirst(query, update, User.class);
    }

    private boolean verifyUser(String userId)
    {
        Authentication authentication = SecurityContextHolder.getContext().getAuthentication();
        String authenticatedEmail = null;
        if (authentication != null && authentication.getPrincipal() instanceof UserDetails) 
            authenticatedEmail = ((UserDetails) authentication.getPrincipal()).getUsername();
        else if (authentication != null && authentication.getPrincipal() instanceof String) 
            authenticatedEmail = (String) authentication.getPrincipal();
        
        if (authenticatedEmail == null) 
            return false;
   
        Query query = new Query(Criteria.where("id").is(userId));
        User user = mongoTemplate.findOne(query, User.class);
        if (user == null) 
            return false;

        if (!authenticatedEmail.equals(user.getMail())) 
            return false;
        
        return true;
    }
}
        



