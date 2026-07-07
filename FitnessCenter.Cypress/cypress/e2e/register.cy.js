describe('FitnessCenter - Register', () => {

  it('успешно регистрира нов корисник', () => {
    const randomEmail = `marko.markovski${Date.now()}@fitness.com`;
    const password = 'Marko!1';

    cy.visit('/Account/Register');

    cy.get('#FirstName').type('Marko');
    cy.get('#LastName').type('Markovski');
    cy.get('#Email').type(randomEmail);
    cy.get('#Password').type(password);
    cy.get('#ConfirmPassword').type(password);
    cy.get('#Height').type('180');
    cy.get('#Weight').type('75');
    cy.get('#Goal').select('0');
    cy.get('#Gender').select('0');

    cy.get('input[type=submit]').click();

    cy.url().should('not.include', '/Account/Register');
  });

  it('прикажува грешки кога полињата се празни', () => {
    cy.visit('/Account/Register');
    cy.get('input[type=submit]').click();

    cy.get('.text-danger').should('exist');
    cy.url().should('include', '/Account/Register');
  });

  it('прикажува грешка кога лозинките не се совпаѓаат', () => {
    const randomEmail = `marko.markovski${Date.now()}@fitness.com`;

    cy.visit('/Account/Register');

    cy.get('#FirstName').type('Marko');
    cy.get('#LastName').type('Markovski');
    cy.get('#Email').type(randomEmail);
    cy.get('#Password').type('Marko!1');
    cy.get('#ConfirmPassword').type('Marko!2');
    cy.get('#Height').type('180');
    cy.get('#Weight').type('75');
    cy.get('#Goal').select('0');
    cy.get('#Gender').select('0');

    cy.get('input[type=submit]').click();

    cy.get('.text-danger').should('contain.text', 'Лозинките не се совпаѓаат');
    cy.url().should('include', '/Account/Register');
  });

});