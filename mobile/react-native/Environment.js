const ENV = {
  dev: {
    apiUrl: 'http://localhost:44368',
    oAuthConfig: {
      issuer: 'http://localhost:44368',
      clientId: 'Blogging_App',
      clientSecret: '1q2w3e*',
      scope: 'Blogging',
    },
    localization: {
      defaultResourceName: 'Blogging',
    },
  },
  prod: {
    apiUrl: 'http://localhost:44368',
    oAuthConfig: {
      issuer: 'http://localhost:44368',
      clientId: 'Blogging_App',
      clientSecret: '1q2w3e*',
      scope: 'Blogging',
    },
    localization: {
      defaultResourceName: 'Blogging',
    },
  },
};

export const getEnvVars = () => {
  // eslint-disable-next-line no-undef
  return __DEV__ ? ENV.dev : ENV.prod;
};
