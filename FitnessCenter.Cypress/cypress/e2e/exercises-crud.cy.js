describe('FitnessCenter - Exercises CRUD', () => {

  const uniqueDescription = `Тест вежба 1783360812470`;

  it('креирање нова вежба', () => {
    cy.visit('/Exercises/Create');

    cy.get('#MuscleGroup').select('0');
    cy.get('#Description').type(uniqueDescription);
    cy.get('#MediaUrl').type('https://www.youtube.com/watch?v=G58Pn_ELIXQ');
    cy.get('input[type=submit]').click();

    cy.url().should('include', '/Exercises');
    cy.contains(uniqueDescription).should('exist');
  });

  it('менување постоечка вежба', () => {
    cy.visit('/Exercises');
    cy.contains(uniqueDescription)
      .parents('tr')
      .find('a').contains('Измени').click();

    const updatedDescription = `${uniqueDescription} - изменето`;
    cy.get('#Description').clear().type(updatedDescription);
    cy.get('input[type=submit]').click();

    cy.url().should('include', '/Exercises');
    cy.contains(updatedDescription).should('exist');
  });

  it('бришење вежба', () => {
    const updatedDescription = `${uniqueDescription} - изменето`;

    cy.visit('/Exercises');
    cy.contains(updatedDescription)
      .parents('tr')
      .find('a').contains('Избриши').click();

    cy.get('input[type=submit]').click();

    cy.url().should('include', '/Exercises');
    cy.contains(updatedDescription).should('not.exist');
  });

});