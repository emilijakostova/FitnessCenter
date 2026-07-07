describe('FitnessCenter - WorkoutPlans CRUD', () => {

  const uniqueNotes = `Тест план ${Date.now()}`;

  const loginAsMario = () => {
    cy.visit('/Account/Login');
    cy.get('#Email').type('mario.ivanov@fitness.com');
    cy.get('#Password').type('Mario!2');
    cy.get('input[type=submit]').click();
    cy.url().should('eq', Cypress.config().baseUrl + '/');
  };

  it('Member (Mario) креира нов workout plan', () => {
    loginAsMario();
    cy.visit('/WorkoutPlans/Create');

    cy.url().should('include', '/WorkoutPlans');

    cy.get('#WorkoutProgramId option').first().then(($opt) => {
      cy.get('#WorkoutProgramId').select($opt.val());
    });

    const today = new Date().toISOString().split('T')[0];
    cy.get('#StrartDate').clear().type(today);
    cy.get('#Notes').clear().type(uniqueNotes);
    cy.get('input[type=submit]').click();

    cy.url().should('include', '/WorkoutPlans');
    cy.contains(uniqueNotes).should('exist');
  });

  it('Member (Mario) го менува сопствениот workout plan', () => {
    loginAsMario();
    cy.visit('/WorkoutPlans');
    cy.contains(uniqueNotes)
      .parents('tr')
      .find('a').contains('Измени').click();

    const today = new Date().toISOString().split('T')[0];
    cy.get('#StrartDate').clear().type(today);

    const updatedNotes = `${uniqueNotes} - изменето`;
    cy.get('#Notes').clear().type(updatedNotes);
    cy.get('input[type=submit]').click();

    cy.url().should('eq', Cypress.config().baseUrl + '/WorkoutPlans');
    cy.contains(updatedNotes).should('exist');
  });

  it('го брише workout planot', () => {
    const updatedNotes = `${uniqueNotes} - изменето`;

    loginAsMario();
    cy.visit('/WorkoutPlans');

    cy.contains(updatedNotes)
      .parents('tr')
      .find('a').contains('Избриши').click();

    cy.get('input[type=submit]').click();

    cy.url().should('include', '/WorkoutPlans');
    cy.contains(updatedNotes).should('not.exist');
  });

});