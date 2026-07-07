describe('FitnessCenter - Login', () => {

  beforeEach(() => {
    cy.visit('/Account/Login');
  });

  it('прикажува login форма', () => {
    cy.get('#Email').should('be.visible');
    cy.get('#Password').should('be.visible');
    cy.get('input[type=submit]').should('have.value', 'Најави се');
  });

  it('неуспешен login со погрешна лозинка ја прикажува грешката', () => {
    cy.get('#Email').type('mona.lisa@fitness.com');
    cy.get('#Password').type('PogresnaLozinka');
    cy.get('input[type=submit]').click();

    cy.get('.text-danger').should('contain.text', 'Invalid login attempt');
    cy.url().should('include', '/Account/Login');
  });

});