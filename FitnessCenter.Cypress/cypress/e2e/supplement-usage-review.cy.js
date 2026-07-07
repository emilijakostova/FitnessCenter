describe('FitnessCenter - Supplement Usage & Review', () => {

  let supplementId;

  const login = (email, password) => {
    cy.visit('/Account/Login');
    cy.get('#Email').type(email);
    cy.get('#Password').type(password);
    cy.get('input[type=submit]').click();
    cy.url().should('eq', Cypress.config().baseUrl + '/');
  };

  it('избира производ и додава употреба', () => {
    login('mario.ivanov@fitness.com', 'Mario!2');

    cy.visit('/Supplements/Browse');
    cy.contains('.category-link', 'Протеини').click();
    cy.url({ timeout: 20000 }).should('include', '/Supplements/BrowseCategory');
    cy.get('table tbody tr', { timeout: 20000 }).should('have.length.greaterThan', 0);
    cy.get('table tbody tr').first().contains('Го користам').click();

    cy.url().should('include', '/SupplementUsages/AddUsage').then((url) => {
      const match = url.match(/supplementId=(\d+)/);
      supplementId = match ? match[1] : null;
    });

    const today = new Date().toISOString().split('T')[0];
    cy.get('#DateStarted').clear().type(today);
    cy.get('#Dosage').type('1 лажица дневно');
    cy.get('#Notes').type('Тест употреба преку Cypress');
    cy.contains('button', 'Зачувај').click();

    cy.url().should('include', '/SupplementUsages/MyUsages');
  });

  it('оставa рецензија за истиот производ', () => {
    login('mario.ivanov@fitness.com', 'Mario!2');
    cy.on('window:confirm', () => true);

    cy.visit('/SupplementUsages/MyUsages');

    cy.get('body').then(($body) => {
      if ($body.find(`.delete-review-btn[data-id="${supplementId}"]`).length > 0) {
        cy.get(`.delete-review-btn[data-id="${supplementId}"]`).first().click();
        cy.wait(1000);
      }
    });

    cy.get(`.rate-btn[data-id="${supplementId}"]`).first().should('exist').click();
    cy.get('#ratingModal').should('be.visible');
    cy.get('#starRating .star').eq(3).click();
    cy.get('textarea[name="Comment"]').type('Многу добар производ, препорачувам!');

    cy.window().then((win) => {
      cy.stub(win, 'alert').as('alertStub');
    });

    cy.get('#ratingForm').submit();
    cy.get('@alertStub').should('have.been.calledWith', 'Рецензијата е зачувана!');
    });

  it('рецензијата се прикажува со ѕвезди по освежување', () => {
    login('mario.ivanov@fitness.com', 'Mario!2');
    cy.visit('/SupplementUsages/MyUsages');

    cy.get(`.delete-review-btn[data-id="${supplementId}"]`).should('have.length.greaterThan', 0);
    cy.get(`.rate-btn[data-id="${supplementId}"]`).should('not.exist');
  });

});