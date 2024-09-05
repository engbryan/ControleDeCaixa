import axios from 'axios';
import * as AmazonCognitoIdentity from 'amazon-cognito-identity-js';

const poolData = {
  UserPoolId: 'us-east-1_U19RVhgkT', // Substitua pelo seu UserPoolId
  ClientId: 'qa00v9u2j3af7lgtk6siku91e', // Substitua pelo seu ClientId
};

const userPool = new AmazonCognitoIdentity.CognitoUserPool(poolData);

export default {
  async login(username, password) {
    const authenticationDetails = new AmazonCognitoIdentity.AuthenticationDetails({
      Username: username,
      Password: password,
    });

    const userData = {
      Username: username,
      Pool: userPool,
    };

    const cognitoUser = new AmazonCognitoIdentity.CognitoUser(userData);

    return new Promise((resolve, reject) => {
      cognitoUser.authenticateUser(authenticationDetails, {
        onSuccess: (result) => {
          const token = result.getIdToken().getJwtToken();
          localStorage.setItem('authToken', token);
          axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
          resolve(result);
        },
        onFailure: (err) => {
          console.error('Erro no login:', err);
          reject('Falha na autenticação');
        },
        newPasswordRequired: (userAttributes, requiredAttributes) => {
          cognitoUser.completeNewPasswordChallenge(password, userAttributes, {
            onSuccess: (result) => {
              const token = result.getIdToken().getJwtToken();
              localStorage.setItem('authToken', token);
              axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
              resolve(result);
            },
            onFailure: (err) => {
              console.error('Erro ao definir nova senha:', err);
              reject('Falha na autenticação');
            }
          });
        },
        mfaRequired: (codeDeliveryDetails) => {
          // Handle MFA requirement here
        },
        mfaSetup: (challengeName, challengeParameters) => {
          // Handle MFA setup here
        },
        totpRequired: (challengeName, challengeParameters) => {
          // Handle TOTP requirement here
        }
      });
    });
  },

  logout() {
    const cognitoUser = userPool.getCurrentUser();
    if (cognitoUser) {
      cognitoUser.signOut();
    }
    localStorage.removeItem('authToken');
    delete axios.defaults.headers.common['Authorization'];
  },

  isAuthenticated() {
    return !!localStorage.getItem('authToken');
  },

  getToken() {
    return localStorage.getItem('authToken');
  }
};
