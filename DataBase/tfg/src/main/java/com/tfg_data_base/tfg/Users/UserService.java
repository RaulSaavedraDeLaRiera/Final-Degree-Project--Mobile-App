package com.tfg_data_base.tfg.Users;

import java.util.List;
import java.util.Map;
import java.security.NoSuchAlgorithmException;
import java.time.Instant;
import java.util.ArrayList;
import java.util.Optional;
import java.util.regex.Pattern;
import java.util.stream.Collectors;
import java.security.MessageDigest;

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
import org.springframework.security.core.userdetails.UserDetails;

import lombok.RequiredArgsConstructor;

@Service
@RequiredArgsConstructor
public class UserService {
    private final UserRepository userRepository;
    private final UserInfoService userInfoService;

    private String passwordToken = "1234567890qrtweu12h32i3o2nr23kj432mbr23kjeg32kjerg32ody2d8cUSUDAUbefgwfu23kweqhf";

    private JwtUtils jwtUtils = new JwtUtils();
    // private String userPasswordCredentials = "Jm7N@q9!Xf2#ZlT6pV";

    @Autowired
    private MongoTemplate mongoTemplate;

    /**
     * Se llama cuando se crea un nuevo usuario
     * 
     * @param user
     */
    public void save(User user) {

        String gmailRegex = "^[a-zA-Z0-9._%+-]+@gmail\\.com$";
        Pattern pattern = Pattern.compile(gmailRegex);
        if (!pattern.matcher(user.getMail()).matches()) {
            throw new IllegalArgumentException("Is not a mail");
        }

        if (userRepository.findByMail(user.getMail()).isPresent()) {
            throw new IllegalArgumentException("Mail in use");
        }

        if (user.getUserData() == null) {
            user.setUserData(new User.UserData());
        }

        if (user.getSocialStats() == null) {
            user.setSocialStats(new ArrayList<>());
        }

        if (user.getPendingSocialStats() == null) {
            user.setPendingSocialStats(new ArrayList<>());
        }

        if (user.getSentSocialStats() == null) {
            user.setSentSocialStats(new ArrayList<>());
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

        if (user.getMail() == null)
            user.setMail("");
        if (user.getPassword() == null)
            user.setPassword("");

        User.UserData userData = user.getUserData();
        if (userData.getName() == null)
            userData.setName("");
        if (userData.getLevel() == null)
            userData.setLevel(0);
        if (userData.getExperience() == null)
            userData.setExperience(0);
        if (userData.getMoney() == null)
            userData.setMoney(0);
        if (userData.getCurrentCritteron() == null)
            userData.setCurrentCritteron("");
        userData.setLastClosedTime(Instant.now().getEpochSecond());

        User.PersonalStats personalStats = user.getPersonalStats();
        if (personalStats.getGlobalSteps() == null)
            personalStats.setGlobalSteps(0);
        if (personalStats.getDaysStreak() == null)
            personalStats.setDaysStreak(0);
        if (personalStats.getWeekSteps() == null)
            personalStats.setWeekSteps(0);
        if (personalStats.getCombatWins() == null)
            personalStats.setCombatWins(0);
        if (personalStats.getCritteronsOwned() == null)
            personalStats.setCritteronsOwned(0);
        if (personalStats.getPercentHotel() == null)
            personalStats.setPercentHotel(0);

        userRepository.save(user);
        userInfoService.addUser(user.getId());
    }

    public String login(String mail, String password) {
        Query query = new Query();
        query.addCriteria(Criteria.where("mail").is(mail));
        query.fields().include("password").include("mail");

        @SuppressWarnings("unchecked")
        Map<String, Object> user = mongoTemplate.findOne(query, Map.class, "User");
        if (user != null && password.equals((String) user.get("password"))) {
            return jwtUtils.generateJwtToken(mail);
        }

        return "";
    }

    public String token(String password) {
        System.out.println("Hash recibido: " + password);
        System.out.println("Contraseña esperado: " + passwordToken);
        String pass = hashPassword(passwordToken);
        System.out.println("Hash esperado: " + pass);


        if (password.equals(pass)) {
            return jwtUtils.generateJwtToken(password);
        }
        return "ERROR";
    }
    

    private static String hashPassword(String password) {
        try {
            MessageDigest digest = MessageDigest.getInstance("SHA-256");
            byte[] hash = digest.digest(password.getBytes());
            StringBuilder hexString = new StringBuilder();
            for (byte b : hash) {
                String hex = Integer.toHexString(0xff & b);
                if (hex.length() == 1) hexString.append('0');
                hexString.append(hex);
            }
            return hexString.toString();
        } catch (NoSuchAlgorithmException e) {
            throw new RuntimeException("Error generando hash SHA-256", e);
        }
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
        if (!verifyUser(id))
            throw new SecurityException("Cant modify a different user");

        userRepository.deleteById(id);
        userInfoService.removeUser(id);
    }

    /**
     * Actualizar un parametro de un usuario en concreto
     * 
     * @param userId
     * @param fieldName
     * @param newValue
     */
    public void updateUserField(String userId, String fieldName, Object newValue) {

        if (!verifyUser(userId))
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
        } else {
            Update update = new Update().set(fieldName, newValue);
            mongoTemplate.updateFirst(query, update, User.class);
        }
    }

    public void removeFriend(String userId, String friendID) {

        if (!verifyUser(userId))
            throw new SecurityException("Cant modify a different user");

        Query query = new Query(Criteria.where("id").is(userId));
        Update update = new Update().pull("socialStats", new User.SocialStat(friendID));
        mongoTemplate.updateFirst(query, update, User.class);
    }

    public void removeFriendPending(String userId, String friendID) {
        Query query = new Query(Criteria.where("id").is(userId));
        Update update = new Update().pull("pendingSocialStats", new User.PendingSocialStat(friendID));
        mongoTemplate.updateFirst(query, update, User.class);
    }

    public void removeFriendSent(String userId, String friendID) {
        Query query = new Query(Criteria.where("id").is(userId));
        Update update = new Update().pull("sentSocialStats", new User.SentSocialStat(friendID));
        mongoTemplate.updateFirst(query, update, User.class);
    }

    public void updateUserSentFriend(String userId, String fieldName, Object newValue) {
        Query query = new Query(Criteria.where("id").is(userId));
        if ("sentSocialStats".equals(fieldName)) {
            String newsentSocialStatID = (String) newValue;
            Update update = new Update().addToSet("sentSocialStats", new User.SentSocialStat(newsentSocialStatID));
            mongoTemplate.updateFirst(query, update, User.class);
        }
    }

    public void updateUserPendingFriend(String userId, String fieldName, Object newValue) {
        Query query = new Query(Criteria.where("id").is(userId));

        if ("pendingSocialStats".equals(fieldName)) {
            String newPendingSocialStatID = (String) newValue;
            Update update = new Update().addToSet("pendingSocialStats",
                    new User.PendingSocialStat(newPendingSocialStatID));
            mongoTemplate.updateFirst(query, update, User.class);
        }
    }

    private boolean verifyUser(String userId) {
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

    public List<String> getTopThreeUsersByLevel() {
        Query query = new Query();
        query.limit(3);
        query.with(org.springframework.data.domain.Sort
                .by(org.springframework.data.domain.Sort.Order.desc("userData.level")));

        List<User> topUsers = mongoTemplate.find(query, User.class);

        return topUsers.stream()
                .map(User::getId)
                .collect(Collectors.toList());
    }

    public List<String> getTopThreeFriendsByLevel(String userId) {
        Optional<User> userOpt = userRepository.findById(userId);
        if (!userOpt.isPresent()) {
            throw new IllegalArgumentException("User not found");
        }

        User user = userOpt.get();
        List<String> friendIds = user.getSocialStats().stream()
                .map(User.SocialStat::getFriendID)
                .collect(Collectors.toList());

        if (friendIds.isEmpty()) {
            return new ArrayList<>();
        }

        Query query = new Query(Criteria.where("id").in(friendIds));
        List<User> friends = mongoTemplate.find(query, User.class);

        List<User> sortedFriends = friends.stream()
                .sorted((u1, u2) -> Integer.compare(u2.getUserData().getLevel(), u1.getUserData().getLevel()))
                .collect(Collectors.toList());
        return sortedFriends.stream()
                .limit(3)
                .map(User::getId)
                .collect(Collectors.toList());
    }
}
