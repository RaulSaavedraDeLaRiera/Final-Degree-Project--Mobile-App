package com.tfg_data_base.tfg.Users;

import java.util.Optional;

import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface UserRepository extends MongoRepository<User, String> {
    Optional<User> findByMail(String mail);

    User findByMailAndPassword(String mail, String password);
}
